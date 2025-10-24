using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.Services
{
    public class RoomService
    {
        private readonly IRoomRepository _roomRepository;

        public RoomService(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public async Task<Room> CreateAsync(
            int number,
            string? name, 
            List<int> seats) 
        {
            var room = new Room(number, name, seats);

            await _roomRepository.CreateAsync(room);

            return room;
        }

        public async Task<Room> GetByIdAsync(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);

            if (room == null)
                return null;

            return room;
        }

        public async Task<Room> UpdateAsync(int id, int number, string? name, List<int> seats)
        {
            var existentRoom = await _roomRepository.GetByIdAsync(id);

            if (existentRoom == null)
                return null;

            existentRoom.Update(number, name, seats);

            await _roomRepository.UpdateAsync(existentRoom);

            return existentRoom;
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);

            if (room == null)
                return false;
            
            await _roomRepository.DeleteByIdAsync(id);

            return true;
        }

        public async Task<Room> GetByNumberAsync(int number)
        {
            var room = await _roomRepository.GetByNumberAsync(number);

            if (room == null)
                return null;

            return room;
        }

        public async Task<IEnumerable<Room>> GetAllAsync()
        {
            var rooms = await _roomRepository.GetAllAsync();

            return rooms ?? Enumerable.Empty<Room>();
        }

    }
}
