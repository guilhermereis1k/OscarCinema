using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OscarCinema.Application.Services
{
    public class SeatService
    {
        private readonly ISeatRepository _seatRepository;
        public SeatService(ISeatRepository seatRepository)
        {
            _seatRepository = seatRepository;
        }

        public async Task<Seat> CreateAsync(int roomId,
            bool isOccupied, 
            char row, 
            int number)
        {
            var seat = new Seat(roomId, isOccupied, row, number);

            await _seatRepository.CreateAsync(seat);
            
            return seat;
        }

        public async Task<Seat?> GetByIdAsync(int id)
        {
            var seat = await _seatRepository.GetByIdAsync(id);

            if (seat == null)
                return null;

            await _seatRepository.DeleteByIdAsync(id);

            return seat;
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var seat = await _seatRepository.GetByIdAsync(id);

            if (seat == null)
                return false;

            await _seatRepository.DeleteByIdAsync(id);

            return true;
        }

        public async Task<Seat?> GetByRowAndNumberAsync(int row, int number)
        {
            var seat = await _seatRepository.GetByRowAndNumberAsync(row, number);

            if (seat == null)
                return null;

            return seat;
        }

        public async Task<IEnumerable<Seat>?> GetSeatsByRoomIdAsync(int roomId)
        {
            var seat = await _seatRepository.GetSeatsByRoomIdAsync(roomId);

            if (seat == null)
                return null;

            return seat;
        }
    }
}
