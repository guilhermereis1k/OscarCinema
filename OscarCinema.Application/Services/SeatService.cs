using AutoMapper;
using OscarCinema.Application.DTOs.Seat;
using OscarCinema.Application.Interfaces;
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
    public class SeatService : ISeatService
    {
        private readonly ISeatRepository _repository;
        private readonly IMapper _mapper;

        public SeatService(ISeatRepository seatRepository, IMapper mapper)
        {
            _repository = seatRepository;
            _mapper = mapper;
        }

        public async Task<SeatResponseDTO> CreateAsync(CreateSeatDTO dto)
        {
            var seat = _mapper.Map<Seat>(dto);
            await _repository.AddAsync(seat);

            return _mapper.Map<SeatResponseDTO>(seat);
        }

        public async Task<SeatResponseDTO?> GetByIdAsync(int id)
        {
            var seat = await _repository.GetByIdAsync(id);
            return seat == null ? null : _mapper.Map<SeatResponseDTO>(seat);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var seat = await _repository.GetByIdAsync(id);
            if (seat == null) return false;

            await _repository.DeleteAsync(id);
            return true;
        }
        public async Task<SeatResponseDTO?> GetByRowAndNumberAsync(char row, int number)
        {
            var seat = await _repository.GetByRowAndNumberAsync(row, number);
            if (seat == null)
                return null;

            return _mapper.Map<SeatResponseDTO>(seat);
        }

        public async Task<IEnumerable<SeatResponseDTO>?> GetSeatsByRoomIdAsync(int roomId)
        {
            var seats = await _repository.GetSeatsByRoomIdAsync(roomId);
            return seats == null ? null : _mapper.Map<IEnumerable<SeatResponseDTO>>(seats);
        }

        public async Task<SeatResponseDTO?> OccupySeatAsync(int id)
        {
            var seat = await _repository.GetByIdAsync(id);
            if (seat == null)
                return null;

            seat.OccupySeat(id);

            await _repository.UpdateAsync(seat);
            return _mapper.Map<SeatResponseDTO>(seat);
        }

        public async Task<SeatResponseDTO?> FreeSeatAsync(int id)
        {
            var seat = await _repository.GetByIdAsync(id);
            if (seat == null)
                return null;

            seat.FreeSeat(id);

            await _repository.UpdateAsync(seat);
            return _mapper.Map<SeatResponseDTO>(seat);
        }
    }
}
