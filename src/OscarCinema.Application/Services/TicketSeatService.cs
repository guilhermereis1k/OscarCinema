using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OscarCinema.Application.DTOs.TicketSeat;
using OscarCinema.Application.Interfaces;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Domain.Validation;

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

    public async Task<TicketSeatResponse> UpdatePriceAsync(int ticketId, int seatId, decimal newPrice)
    {
        _logger.LogInformation("Updating ticket seat price - TicketId: {TicketId}, SeatId: {SeatId}", ticketId, seatId);

        var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(ticketId)
            ?? throw new DomainExceptionValidation("Ticket not found.");

        var seat = ticket.TicketSeats.FirstOrDefault(ts => ts.SeatId == seatId)
            ?? throw new DomainExceptionValidation("TicketSeat not found.");

        seat.UpdatePrice(newPrice);

        await _unitOfWork.TicketRepository.UpdateAsync(ticket);
        await _unitOfWork.CommitAsync();

        return _mapper.Map<TicketSeatResponse>(seat);
    }
}