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
        private readonly ISeatRepository _seatRepository;
        private readonly IMapper _mapper;

        public SeatService(ISeatRepository seatRepository, IMapper mapper)
        {
            _seatRepository = seatRepository;
            _mapper = mapper;
        }

        public async Task<SeatResponseDTO> CreateAsync(CreateSeatDTO dto)
        {
            var seat = _mapper.Map<Seat>(dto);
            await _seatRepository.CreateAsync(seat);

            return _mapper.Map<SeatResponseDTO>(seat);
        }

        public async Task<SeatResponseDTO?> GetByIdAsync(int id)
        {
            var seat = await _seatRepository.GetByIdAsync(id);
            return seat == null ? null : _mapper.Map<SeatResponseDTO>(seat);
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var seat = await _seatRepository.GetByIdAsync(id);
            if (seat == null) return false;

            await _seatRepository.DeleteByIdAsync(id);
            return true;
        }
        public async Task<SeatResponseDTO?> GetByRowAndNumberAsync(char row, int number)
        {
            var seat = await _seatRepository.GetByRowAndNumberAsync(row, number);
            if (seat == null)
                return null;

            return _mapper.Map<SeatResponseDTO>(seat);
        }

        public async Task<IEnumerable<SeatResponseDTO>?> GetSeatsByRoomIdAsync(int roomId)
        {
            var seats = await _seatRepository.GetSeatsByRoomIdAsync(roomId);
            return seats == null ? null : _mapper.Map<IEnumerable<SeatResponseDTO>>(seats);
        }
    }
}
