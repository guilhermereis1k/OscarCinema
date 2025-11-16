using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using OscarCinema.Application.DTOs.Pagination;
using OscarCinema.Application.DTOs.Room;
using OscarCinema.Application.Interfaces;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.Services
{
    public class RoomService : IRoomService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<RoomService> _logger;

        public RoomService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<RoomService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<RoomResponse> CreateAsync(CreateRoom dto)
        {
            _logger.LogInformation("Creating new room: {RoomName} (Number: {RoomNumber}) with {SeatCount} seats",
                dto.Name, dto.Number, dto.Seats.Count);

            var room = new Room(dto.Number, dto.Name);

            foreach (var seatDto in dto.Seats)
            {
                var seat = new Seat(seatDto.Row, seatDto.Number, false, seatDto.SeatTypeId);
                room.AddSeat(seat);
            }

            await _unitOfWork.RoomRepository.AddAsync(room);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Room created successfully: {RoomName} (ID: {RoomId}) with {SeatCount} seats",
                room.Name, room.Id, room.Seats.Count);
            return _mapper.Map<RoomResponse>(room);
        }

        public async Task<RoomResponse?> GetByIdAsync(int id)
        {
            _logger.LogDebug("Getting room by ID: {RoomId}", id);

            var entity = await _unitOfWork.RoomRepository.GetByIdAsync(id);

            if (entity == null)
            {
                _logger.LogWarning("Room not found: {RoomId}", id);
                return null;
            }

            _logger.LogDebug("Room found: {RoomName} (ID: {RoomId}) with {SeatCount} seats",
                entity.Name, id, entity.Seats.Count);
            return _mapper.Map<RoomResponse>(entity);
        }

        public async Task<RoomResponse?> UpdateAsync(int id, UpdateRoom dto)
        {
            _logger.LogInformation("Updating room ID: {RoomId} with {SeatCount} seats", id, dto.Seats.Count);

            var existentRoom = await _unitOfWork.RoomRepository.GetByIdAsync(id);
            if (existentRoom == null)
            {
                _logger.LogWarning("Room not found for update: {RoomId}", id);
                return null;
            }

            existentRoom.Update(dto.Number, dto.Name);

            var updatedSeats = new List<Seat>();

            foreach (var seatDto in dto.Seats)
            {
                var existingSeat = existentRoom.Seats.FirstOrDefault(s => s.Id == seatDto.Id);

                if (existingSeat != null)
                {
                    existingSeat.Update(seatDto.Id, seatDto.Row, seatDto.Number, seatDto.IsOccupied, seatDto.SeatTypeId);
                    updatedSeats.Add(existingSeat);
                }
                else
                {
                    var newSeat = new Seat(seatDto.Row, seatDto.Number, seatDto.IsOccupied, seatDto.SeatTypeId);
                    updatedSeats.Add(newSeat);
                }
            }

            var seatsToRemove = existentRoom.Seats
                .Where(s => !dto.Seats.Any(dtoSeat => dtoSeat.Id == s.Id))
                .ToList();

            foreach (var seat in seatsToRemove)
                existentRoom.RemoveSeat(seat);

            existentRoom.SetSeats(updatedSeats);

            await _unitOfWork.RoomRepository.UpdateAsync(existentRoom);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Room updated successfully: {RoomName} (ID: {RoomId}) with {SeatCount} seats",
                existentRoom.Name, id, existentRoom.Seats.Count);
            return _mapper.Map<RoomResponse>(existentRoom);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting room: {RoomId}", id);

            var entity = await _unitOfWork.RoomRepository.GetByIdAsync(id);
            if (entity == null)
            {
                _logger.LogWarning("Room not found for deletion: {RoomId}", id);
                return false;
            }

            await _unitOfWork.RoomRepository.DeleteAsync(id);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Room deleted successfully: {RoomId}", id);
            return true;
        }

        public async Task<PaginationResult<RoomResponse>> GetAllAsync(PaginationQuery query)
        {
            _logger.LogDebug("Getting all rooms with pagination");

            var baseQuery = _unitOfWork.RoomRepository.GetAllQueryable();

            var totalItems = await baseQuery.CountAsync();

            var rooms = await baseQuery
                .OrderBy(r => r.Id)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            var roomDtos = _mapper.Map<IEnumerable<RoomResponse>>(rooms);

            _logger.LogDebug("Retrieved {RoomCount} rooms", roomDtos.Count());

            return new PaginationResult<RoomResponse>
            {
                CurrentPage = query.PageNumber,
                PageSize = query.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize),
                Data = roomDtos
            };
        }

        public async Task<RoomResponse?> GetByNumberAsync(int number)
        {
            _logger.LogDebug("Getting room by number: {RoomNumber}", number);

            var entity = await _unitOfWork.RoomRepository.GetByNumberAsync(number);

            if (entity == null)
            {
                _logger.LogWarning("Room not found with number: {RoomNumber}", number);
                return null;
            }

            _logger.LogDebug("Room found by number: {RoomName} (Number: {RoomNumber})", entity.Name, number);
            return _mapper.Map<RoomResponse>(entity);
        }

        public async Task<RoomResponse?> AddSeatsAsync(int roomId, AddSeatsToRoom dto)
        {
            _logger.LogInformation("Adding {SeatCount} seats to room ID: {RoomId}", dto.Seats.Count, roomId);

            var entity = await _unitOfWork.RoomRepository.GetByIdAsync(roomId);
            if (entity == null)
            {
                _logger.LogWarning("Room not found for adding seats: {RoomId}", roomId);
                return null;
            }

            var newSeats = _mapper.Map<IEnumerable<Seat>>(dto.Seats);
            entity.AddSeats(newSeats);

            await _unitOfWork.RoomRepository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Added {SeatCount} seats to room ID: {RoomId}. Total seats now: {TotalSeats}",
                dto.Seats.Count, roomId, entity.Seats.Count);
            return _mapper.Map<RoomResponse>(entity);
        }
    }
}