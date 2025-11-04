using AutoMapper;
using OscarCinema.Application.DTOs.Ticket;
using OscarCinema.Application.DTOs.TicketSeat;
using OscarCinema.Application.Interfaces;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Enums.Ticket;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Domain.Validation;
using OscarCinema.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.Services
{
    public class TicketService : ITicketService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITicketSeatService _ticketSeatService;
        private readonly IPricingService _pricingService;

        public TicketService(IUnitOfWork unitOfWork,
            ITicketSeatService ticketSeatService,
            IPricingService pricingService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _ticketSeatService = ticketSeatService;
            _pricingService = pricingService;
            _mapper = mapper;
        }

        public async Task<TicketResponseDTO> CreateAsync(CreateTicketDTO dto)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(dto.SessionId);
            DomainExceptionValidation.When(session == null, "Session not found");

            var ticket = new Ticket(
                dto.Date,
                dto.UserId,
                session.MovieId,
                session.RoomId,
                dto.SessionId,
                dto.Method,
                dto.PaymentStatus,
                dto.Paid
            );

            await _unitOfWork.TicketRepository.AddAsync(ticket);
            await _unitOfWork.CommitAsync();

            foreach (var seatDto in dto.TicketSeats)
            {
                var seat = await _unitOfWork.SeatRepository.GetByIdAsync(seatDto.SeatId);

                var price = _pricingService.CalculateSeatPrice(
                    session.ExhibitionType,
                    seat.SeatType
                );

                var ticketSeat = new TicketSeat(
                    ticketId: ticket.Id,
                    seatId: seatDto.SeatId,
                    type: seatDto.Type,
                    price: price
                );

                ticket.AddTicketSeat(ticketSeat);
                await _unitOfWork.TicketSeatRepository.AddAsync(ticketSeat);
            }

            ticket.CalculateTotalFromSeats();

            await _unitOfWork.TicketRepository.UpdateAsync(ticket);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<TicketResponseDTO>(ticket);
        }

        public async Task<TicketResponseDTO?> UpdateAsync(int id, UpdateTicketDTO dto)
        {
            var existentTicket = await _unitOfWork.TicketRepository.GetByIdAsync(id);
            if (existentTicket == null)
                return null;

            existentTicket.Update(
                dto.Date,
                dto.UserId,
                dto.MovieId,
                dto.RoomId,
                dto.SessionId,
                dto.Method,
                dto.PaymentStatus,
                dto.Paid
            );

            await _unitOfWork.TicketRepository.UpdateAsync(existentTicket);
            await _unitOfWork.CommitAsync();

            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(id);
            if (ticket == null)
                return false;

            await _unitOfWork.TicketRepository.DeleteAsync(id);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<TicketResponseDTO?> GetByIdAsync(int id)
        {
            var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(id);
            if (ticket == null)
                return null;

            var response = _mapper.Map<TicketResponseDTO>(ticket);
            response.TicketSeats = await _ticketSeatService.GetByTicketIdAsync(id);

            return response;
        }

        public async Task<IEnumerable<TicketResponseDTO>> GetAllAsync()
        {
            var tickets = await _unitOfWork.TicketRepository.GetAllAsync();
            var response = new List<TicketResponseDTO>();

            foreach (var ticket in tickets)
            {
                var ticketDto = _mapper.Map<TicketResponseDTO>(ticket);
                ticketDto.TicketSeats = await _ticketSeatService.GetByTicketIdAsync(ticket.Id);
                response.Add(ticketDto);
            }

            return response;
        }

        public async Task<IEnumerable<TicketResponseDTO>> GetAllByUserIdAsync(int userId)
        {
            var tickets = await _unitOfWork.TicketRepository.GetAllByUserIdAsync(userId);
            var response = new List<TicketResponseDTO>();

            foreach (var ticket in tickets)
            {
                var ticketDto = _mapper.Map<TicketResponseDTO>(ticket);
                ticketDto.TicketSeats = await _ticketSeatService.GetByTicketIdAsync(ticket.Id);
                response.Add(ticketDto);
            }

            return response;
        }

        public async Task<IEnumerable<TicketResponseDTO>> GetAllBySessionIdAsync(int sessionId)
        {
            var tickets = await _unitOfWork.TicketRepository.GetAllBySessionId(sessionId);
            var response = new List<TicketResponseDTO>();

            foreach (var ticket in tickets)
            {
                var ticketDto = _mapper.Map<TicketResponseDTO>(ticket);
                ticketDto.TicketSeats = await _ticketSeatService.GetByTicketIdAsync(ticket.Id);
                response.Add(ticketDto);
            }

            return response;
        }
    }
}
