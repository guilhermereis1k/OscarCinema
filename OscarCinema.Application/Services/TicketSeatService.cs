using AutoMapper;
using OscarCinema.Application.DTOs.TicketSeat;
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
    public class TicketSeatService : ITicketSeatService
    {
        private readonly ITicketSeatRepository _repository;
        private readonly IMapper _mapper;

        public TicketSeatService(ITicketSeatRepository ticketSeatRepository, IMapper mapper)
        {
            _repository = ticketSeatRepository;
            _mapper = mapper;
        }

        public async Task<TicketSeatResponseDTO> CreateAsync(CreateTicketSeatDTO dto)
        {
            var ticketSeat = new TicketSeat(dto.TicketId, dto.SeatId, dto.Type, dto.Price);
            await _repository.AddAsync(ticketSeat);
            return _mapper.Map<TicketSeatResponseDTO>(ticketSeat);
        }

        public async Task<IEnumerable<TicketSeatResponseDTO>> CreateMultipleAsync(IEnumerable<CreateTicketSeatDTO> dtos)
        {
            var ticketSeats = dtos.Select(dto =>
                new TicketSeat(dto.TicketId, dto.SeatId, dto.Type, dto.Price)
            ).ToList();

            await _repository.CreateRangeAsync(ticketSeats);
            return _mapper.Map<IEnumerable<TicketSeatResponseDTO>>(ticketSeats);
        }

        public async Task<TicketSeatResponseDTO?> GetByIdAsync(int id)
        {
            var ticketSeat = await _repository.GetByIdAsync(id);
            return ticketSeat == null ? null : _mapper.Map<TicketSeatResponseDTO>(ticketSeat);
        }

        public async Task<IEnumerable<TicketSeatResponseDTO>> GetByTicketIdAsync(int ticketId)
        {
            var ticketSeats = await _repository.GetByTicketIdAsync(ticketId);
            return _mapper.Map<IEnumerable<TicketSeatResponseDTO>>(ticketSeats);
        }

        public async Task<IEnumerable<TicketSeatResponseDTO>> GetBySeatIdAsync(int seatId)
        {
            var ticketSeats = await _repository.GetBySeatIdAsync(seatId);
            return _mapper.Map<IEnumerable<TicketSeatResponseDTO>>(ticketSeats);
        }

        public async Task<TicketSeatResponseDTO?> UpdatePriceAsync(int id, decimal newPrice)
        {
            var ticketSeat = await _repository.GetByIdAsync(id);
            if (ticketSeat == null)
                return null;

            ticketSeat.UpdatePrice(newPrice);
            await _repository.UpdateAsync(ticketSeat);

            return _mapper.Map<TicketSeatResponseDTO>(ticketSeat);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var ticketSeat = await _repository.GetByIdAsync(id);
            if (ticketSeat == null)
                return false;

            await _repository.DeleteAsync(id);
            return true;
        }

        public async Task<decimal> CalculateTicketTotalAsync(int ticketId)
        {
            var ticketSeats = await _repository.GetByTicketIdAsync(ticketId);
            return ticketSeats.Sum(ts => ts.Price);
        }
    }
}
