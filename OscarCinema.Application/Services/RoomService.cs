using AutoMapper;
using OscarCinema.Application.DTOs.Movie;
using OscarCinema.Application.DTOs.Room;
using OscarCinema.Application.Interfaces;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Interfaces;
using System;
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

        public RoomService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<RoomResponseDTO> CreateAsync(CreateRoomDTO dto)
        {
            var room = new Room(dto.Number, dto.Name);

            foreach (var seatDto in dto.Seats)
            {
                var seat = new Seat(seatDto.Row, seatDto.Number, false, seatDto.SeatTypeId);
                room.AddSeat(seat);
            }

            await _unitOfWork.RoomRepository.AddAsync(room);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<RoomResponseDTO>(room);
        }

        public async Task<RoomResponseDTO?> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.RoomRepository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<RoomResponseDTO>(entity);
        }

        public async Task<RoomResponseDTO?> UpdateAsync(int id, UpdateRoomDTO dto)
        {
            var existentRoom = await _unitOfWork.RoomRepository.GetByIdAsync(id);
            if (existentRoom == null)
                return null;

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

            return _mapper.Map<RoomResponseDTO>(existentRoom);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.RoomRepository.GetByIdAsync(id);
            if (entity == null) return false;
            
            await _unitOfWork.RoomRepository.DeleteAsync(id);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<IEnumerable<RoomResponseDTO>> GetAllAsync()
        {
            var entity = await _unitOfWork.RoomRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<RoomResponseDTO>>(entity);
        }

        public async Task<RoomResponseDTO?> GetByNumberAsync(int number)
        {
            var entity = await _unitOfWork.RoomRepository.GetByNumberAsync(number);

            if (entity == null)
                return null;

            return _mapper.Map<RoomResponseDTO>(entity);
        }

        public async Task<RoomResponseDTO?> AddSeatsAsync(int roomId, AddSeatsToRoomDTO dto)
        {
            var entity = await _unitOfWork.RoomRepository.GetByIdAsync(roomId);
            if (entity == null)
                return null;

            var newSeats = _mapper.Map<IEnumerable<Seat>>(dto.Seats);
            entity.AddSeats(newSeats);

            await _unitOfWork.RoomRepository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<RoomResponseDTO>(entity);
        }
    }
}
