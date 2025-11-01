using AutoMapper;
using OscarCinema.Application.DTOs.TicketSeat;
using OscarCinema.Application.Interfaces;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.Services
{
    public class TicketSeatService : ITicketSeatService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TicketSeatService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<TicketSeatResponseDTO> CreateAsync(CreateTicketSeatDTO dto)
        {
            var ticketSeat = new TicketSeat(dto.TicketId, dto.SeatId, dto.Type, dto.Price);
            await _unitOfWork.TicketSeatRepository.AddAsync(ticketSeat);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<TicketSeatResponseDTO>(ticketSeat);
        }

        public async Task<IEnumerable<TicketSeatResponseDTO>> CreateMultipleAsync(IEnumerable<CreateTicketSeatDTO> dtos)
        {
            var ticketSeats = dtos.Select(dto =>
                new TicketSeat(dto.TicketId, dto.SeatId, dto.Type, dto.Price)
            ).ToList();

            await _unitOfWork.TicketSeatRepository.CreateRangeAsync(ticketSeats);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<IEnumerable<TicketSeatResponseDTO>>(ticketSeats);
        }

        public async Task<TicketSeatResponseDTO?> GetByIdAsync(int id)
        {
            var ticketSeat = await _unitOfWork.TicketSeatRepository.GetByIdAsync(id);
            return ticketSeat == null ? null : _mapper.Map<TicketSeatResponseDTO>(ticketSeat);
        }

        public async Task<IEnumerable<TicketSeatResponseDTO>> GetByTicketIdAsync(int ticketId)
        {
            var ticketSeats = await _unitOfWork.TicketSeatRepository.GetByTicketIdAsync(ticketId);
            return _mapper.Map<IEnumerable<TicketSeatResponseDTO>>(ticketSeats);
        }

        public async Task<IEnumerable<TicketSeatResponseDTO>> GetBySeatIdAsync(int seatId)
        {
            var ticketSeats = await _unitOfWork.TicketSeatRepository.GetBySeatIdAsync(seatId);
            return _mapper.Map<IEnumerable<TicketSeatResponseDTO>>(ticketSeats);
        }

        public async Task<TicketSeatResponseDTO?> UpdatePriceAsync(int id, decimal newPrice)
        {
            var ticketSeat = await _unitOfWork.TicketSeatRepository.GetByIdAsync(id);
            if (ticketSeat == null)
                return null;

            ticketSeat.UpdatePrice(newPrice);
            await _unitOfWork.TicketSeatRepository.UpdateAsync(ticketSeat);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<TicketSeatResponseDTO>(ticketSeat);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var ticketSeat = await _unitOfWork.TicketSeatRepository.GetByIdAsync(id);
            if (ticketSeat == null)
                return false;

            await _unitOfWork.TicketSeatRepository.DeleteAsync(id);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<decimal> CalculateTicketTotalAsync(int ticketId)
        {
            var ticketSeats = await _unitOfWork.TicketSeatRepository.GetByTicketIdAsync(ticketId);
            return ticketSeats.Sum(ts => ts.Price);
        }
    }
}
