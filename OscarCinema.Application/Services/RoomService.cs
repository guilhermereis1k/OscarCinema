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
        private readonly IRoomRepository _roomRepository;
        private readonly IMapper _mapper;

        public RoomService(IRoomRepository roomRepository, IMapper mapper)
        {
            _roomRepository = roomRepository;
            _mapper = mapper;
        }

        public async Task<RoomResponseDTO> CreateAsync(CreateRoomDTO dto) 
        {
            var room = _mapper.Map<Room>(dto);

            await _roomRepository.CreateAsync(room);

            return _mapper.Map<RoomResponseDTO>(room);
        }

        public async Task<RoomResponseDTO?> GetByIdAsync(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            return room == null ? null : _mapper.Map<RoomResponseDTO>(room);
        }

        public async Task<RoomResponseDTO?> UpdateAsync(int id, UpdateRoomDTO dto)
        {
            var existentRoom = await _roomRepository.GetByIdAsync(id);

            if (existentRoom == null)
                return null;

            existentRoom.Update(
                dto.Number,
                dto.Name,
                dto.Seats
            );

            await _roomRepository.UpdateAsync(existentRoom);

            return _mapper.Map<RoomResponseDTO>(existentRoom);
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null) return false;
            
            await _roomRepository.DeleteByIdAsync(id);

            return true;
        }

        public async Task<RoomResponseDTO?> GetByNumberAsync(int number)
        {
            var room = await _roomRepository.GetByNumberAsync(number);

            if (room == null)
                return null;

            return _mapper.Map<RoomResponseDTO>(room);
        }

        public async Task<IEnumerable<RoomResponseDTO>> GetAllAsync()
        {
            var rooms = await _roomRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<RoomResponseDTO>>(rooms);
        }

    }
}
