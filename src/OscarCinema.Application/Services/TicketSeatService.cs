using AutoMapper;
using Microsoft.Extensions.Logging;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TicketSeatService> _logger;

        public TicketSeatService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TicketSeatService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<TicketSeatResponse> CreateAsync(CreateTicketSeat dto)
        {
            _logger.LogInformation("Creating ticket seat for ticket {TicketId} and seat {SeatId} with price {Price}",
                dto.TicketId, dto.SeatId, dto.Price);

            var ticketSeat = new TicketSeat(dto.TicketId, dto.SeatId, dto.Type, dto.Price);
            await _unitOfWork.TicketSeatRepository.AddAsync(ticketSeat);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Ticket seat created successfully: ID {TicketSeatId} for ticket {TicketId}",
                ticketSeat.Id, dto.TicketId);
            return _mapper.Map<TicketSeatResponse>(ticketSeat);
        }

        public async Task<IEnumerable<TicketSeatResponse>> CreateMultipleAsync(IEnumerable<CreateTicketSeat> dtos)
        {
            _logger.LogInformation("Creating multiple ticket seats. Count: {Count}", dtos.Count());

            var ticketSeats = dtos.Select(dto =>
                new TicketSeat(dto.TicketId, dto.SeatId, dto.Type, dto.Price)
            ).ToList();

            await _unitOfWork.TicketSeatRepository.CreateRangeAsync(ticketSeats);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Multiple ticket seats created successfully. Total created: {Count}", ticketSeats.Count);
            return _mapper.Map<IEnumerable<TicketSeatResponse>>(ticketSeats);
        }

        public async Task<TicketSeatResponse?> GetByIdAsync(int id)
        {
            _logger.LogDebug("Getting ticket seat by ID: {TicketSeatId}", id);

            var ticketSeat = await _unitOfWork.TicketSeatRepository.GetByIdAsync(id);

            if (ticketSeat == null)
            {
                _logger.LogWarning("Ticket seat not found: {TicketSeatId}", id);
                return null;
            }

            _logger.LogDebug("Ticket seat found: ID {TicketSeatId} for ticket {TicketId}", id, ticketSeat.TicketId);
            return _mapper.Map<TicketSeatResponse>(ticketSeat);
        }

        public async Task<IEnumerable<TicketSeatResponse>> GetByTicketIdAsync(int ticketId)
        {
            _logger.LogDebug("Getting ticket seats for ticket ID: {TicketId}", ticketId);

            var ticketSeats = await _unitOfWork.TicketSeatRepository.GetByTicketIdAsync(ticketId);
            var result = _mapper.Map<IEnumerable<TicketSeatResponse>>(ticketSeats);

            _logger.LogDebug("Retrieved {Count} ticket seats for ticket ID: {TicketId}", result.Count(), ticketId);
            return result;
        }

        public async Task<IEnumerable<TicketSeatResponse>> GetBySeatIdAsync(int seatId)
        {
            _logger.LogDebug("Getting ticket seats for seat ID: {SeatId}", seatId);

            var ticketSeats = await _unitOfWork.TicketSeatRepository.GetBySeatIdAsync(seatId);
            var result = _mapper.Map<IEnumerable<TicketSeatResponse>>(ticketSeats);

            _logger.LogDebug("Retrieved {Count} ticket seats for seat ID: {SeatId}", result.Count(), seatId);
            return result;
        }

        public async Task<TicketSeatResponse?> UpdatePriceAsync(int id, decimal newPrice)
        {
            _logger.LogInformation("Updating price for ticket seat ID: {TicketSeatId} to {NewPrice}", id, newPrice);

            var ticketSeat = await _unitOfWork.TicketSeatRepository.GetByIdAsync(id);
            if (ticketSeat == null)
            {
                _logger.LogWarning("Ticket seat not found for price update: {TicketSeatId}", id);
                return null;
            }

            ticketSeat.UpdatePrice(newPrice);
            await _unitOfWork.TicketSeatRepository.UpdateAsync(ticketSeat);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Ticket seat price updated successfully: ID {TicketSeatId}", id);
            return _mapper.Map<TicketSeatResponse>(ticketSeat);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting ticket seat: {TicketSeatId}", id);

            var ticketSeat = await _unitOfWork.TicketSeatRepository.GetByIdAsync(id);
            if (ticketSeat == null)
            {
                _logger.LogWarning("Ticket seat not found for deletion: {TicketSeatId}", id);
                return false;
            }

            await _unitOfWork.TicketSeatRepository.DeleteAsync(id);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Ticket seat deleted successfully: {TicketSeatId}", id);
            return true;
        }

        public async Task<decimal> CalculateTicketTotalAsync(int ticketId)
        {
            _logger.LogDebug("Calculating total for ticket ID: {TicketId}", ticketId);

            var ticketSeats = await _unitOfWork.TicketSeatRepository.GetByTicketIdAsync(ticketId);
            var total = ticketSeats.Sum(ts => ts.Price);

            _logger.LogDebug("Calculated total for ticket ID {TicketId}: {Total}", ticketId, total);
            return total;
        }
    }
}