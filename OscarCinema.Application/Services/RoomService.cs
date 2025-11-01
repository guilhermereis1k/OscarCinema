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
        private readonly IRoomRepository _repository;
        private readonly IMapper _mapper;

        public RoomService(IRoomRepository roomRepository, IMapper mapper)
        {
            _repository = roomRepository;
            _mapper = mapper;
        }

        public async Task<RoomResponseDTO> CreateAsync(CreateRoomDTO dto) 
        {
            var room = _mapper.Map<Room>(dto);

            await _repository.AddAsync(room);

            return _mapper.Map<RoomResponseDTO>(room);
        }

        public async Task<RoomResponseDTO?> GetByIdAsync(int id)
        {
            var room = await _repository.GetByIdAsync(id);
            return room == null ? null : _mapper.Map<RoomResponseDTO>(room);
        }

        public async Task<RoomResponseDTO?> UpdateAsync(int id, UpdateRoomDTO dto)
        {
            var existentRoom = await _repository.GetByIdAsync(id);
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

            await _repository.UpdateAsync(existentRoom);

            return _mapper.Map<RoomResponseDTO>(existentRoom);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var room = await _repository.GetByIdAsync(id);
            if (room == null) return false;
            
            await _repository.DeleteAsync(id);

            return true;
        }

        public async Task<RoomResponseDTO?> GetByNumberAsync(int number)
        {
            var room = await _repository.GetByNumberAsync(number);

            if (room == null)
                return null;

            return _mapper.Map<RoomResponseDTO>(room);
        }

        public async Task<IEnumerable<RoomResponseDTO>> GetAllAsync()
        {
            var rooms = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<RoomResponseDTO>>(rooms);
        }

        public async Task<RoomResponseDTO?> AddSeatsAsync(int roomId, AddSeatsToRoomDTO dto)
        {
            var room = await _repository.GetByIdAsync(roomId);
            if (room == null)
                return null;

            var newSeats = _mapper.Map<IEnumerable<Seat>>(dto.Seats);
            room.AddSeats(newSeats);

            await _repository.UpdateAsync(room);
            return _mapper.Map<RoomResponseDTO>(room);
        }
    }
}
