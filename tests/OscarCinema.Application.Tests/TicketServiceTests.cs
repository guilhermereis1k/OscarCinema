using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OscarCinema.Application.DTOs.Pagination;
using OscarCinema.Application.DTOs.Ticket;
using OscarCinema.Application.Interfaces;
using OscarCinema.Application.Services;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Enums.Ticket;
using OscarCinema.Domain.Interfaces;
using Xunit;

namespace OscarCinema.Application.Tests
{
    public class TicketServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<TicketService>> _loggerMock;
        private readonly Mock<IPricingService> _pricingServiceMock;
        private readonly TicketService _service;

        public TicketServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<TicketService>>();
            _pricingServiceMock = new Mock<IPricingService>();

            _service = new TicketService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _loggerMock.Object,
                _pricingServiceMock.Object
            );
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnDto_WhenTicketExists()
        {
            var ticket = new Ticket(1, 1, 1, 1, PaymentMethod.CreditCard);

            var ticketDto = new TicketResponse
            {
                Id = 1,
                UserId = 1,
                SessionId = 1
            };

            _unitOfWorkMock
                .Setup(u => u.TicketRepository.GetByIdAsync(1))
                .ReturnsAsync(ticket);

            _mapperMock
                .Setup(m => m.Map<TicketResponse>(ticket))
                .Returns(ticketDto);

            var result = await _service.GetByIdAsync(1);

            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.UserId.Should().Be(1);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenTicketDoesNotExist()
        {
            _unitOfWorkMock
                .Setup(u => u.TicketRepository.GetByIdAsync(1))
                .ReturnsAsync((Ticket?)null);

            var result = await _service.GetByIdAsync(1);

            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateTicket_AndReturnDto()
        {
            var createDto = new CreateTicket
            {
                SessionId = 1,
                UserId = 1,
                Method = PaymentMethod.CreditCard,
                Seats = new List<SeatSelection>
        {
            new SeatSelection { SeatId = 1, Type = TicketType.Full}
        }
            };

            var startTime = DateTime.Now.AddHours(1);
            var durationMinutes = 120;

            var session = new Session(
                movieId: 1,
                roomId: 1,
                exhibitionTypeId: 1,
                startTime: startTime,
                durationMinutes: durationMinutes
            );

            var sessionIdProperty = session.GetType().GetProperty("Id");
            if (sessionIdProperty != null && sessionIdProperty.CanWrite)
            {
                sessionIdProperty.SetValue(session, 1);
            }

            var room = new Room(1, "Sala 1");

            var seat = new Seat(
                roomId: 1,
                row: 'A',
                number: 1,
                seatTypeId: 1
            );

            var seatIdProperty = seat.GetType().GetProperty("Id");
            if (seatIdProperty != null && seatIdProperty.CanWrite)
            {
                seatIdProperty.SetValue(seat, 1);
            }

            room.AddSeat(seat);

            var roomProperty = session.GetType().GetProperty("Room");
            if (roomProperty != null && roomProperty.CanWrite)
            {
                roomProperty.SetValue(session, room);
            }

            _pricingServiceMock
                .Setup(p => p.CalculateSeatPriceAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(20.0m);

            _pricingServiceMock
                .Setup(p => p.ApplyTicketType(It.IsAny<decimal>(), It.IsAny<TicketType>()))
                .Returns<decimal, TicketType>((basePrice, type) => basePrice);

            _unitOfWorkMock
                .Setup(u => u.SessionRepository.GetDetailedAsync(1))
                .ReturnsAsync(session);

            _unitOfWorkMock
                .Setup(u => u.TicketRepository.AddAsync(It.IsAny<Ticket>()))
                .Returns(Task.CompletedTask)
                .Callback<Ticket>(ticket =>
                {
                    var idProperty = ticket.GetType().GetProperty("Id");
                    if (idProperty != null && idProperty.CanWrite)
                    {
                        idProperty.SetValue(ticket, 1);
                    }
                });

            _unitOfWorkMock
                .Setup(u => u.SessionRepository.UpdateAsync(session))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.CommitAsync())
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.TicketRepository.GetDetailedAsync(1))
                .ReturnsAsync(() =>
                {
                    var ticket = new Ticket(
                        userId: 1,
                        movieId: 1,
                        roomId: 1,
                        sessionId: 1,
                        method: PaymentMethod.CreditCard
                    );

                    var idProperty = ticket.GetType().GetProperty("Id");
                    if (idProperty != null && idProperty.CanWrite)
                    {
                        idProperty.SetValue(ticket, 1);
                    }

                    var ticketSeat = new TicketSeat(
                        seatId: 1,
                        type: TicketType.Full,
                        price: 20.0m
                    );

                    var ticketSeatIdProperty = ticketSeat.GetType().GetProperty("Id");
                    if (ticketSeatIdProperty != null && ticketSeatIdProperty.CanWrite)
                    {
                        ticketSeatIdProperty.SetValue(ticketSeat, 1);
                    }

                    var ticketIdProperty = ticketSeat.GetType().GetProperty("TicketId");
                    if (ticketIdProperty != null && ticketIdProperty.CanWrite)
                    {
                        ticketIdProperty.SetValue(ticketSeat, 1);
                    }

                    var ticketSeatProperty = ticket.GetType().GetProperty("TicketSeats");
                    if (ticketSeatProperty != null && ticketSeatProperty.CanWrite)
                    {
                        var ticketSeatsList = new List<TicketSeat> { ticketSeat };
                        ticketSeatProperty.SetValue(ticket, ticketSeatsList);
                    }

                    return ticket;
                });

            _mapperMock
                .Setup(m => m.Map<TicketResponse>(It.IsAny<Ticket>()))
                .Returns(new TicketResponse { Id = 1 });

            var result = await _service.CreateAsync(createDto);

            result.Should().NotBeNull();
            result.Id.Should().Be(1);

            _unitOfWorkMock.Verify(u => u.TicketRepository.AddAsync(It.IsAny<Ticket>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SessionRepository.UpdateAsync(session), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenTicketExists()
        {
            var ticket = new Ticket(1, 1, 1, 1, PaymentMethod.CreditCard);

            _unitOfWorkMock
                .Setup(u => u.TicketRepository.GetByIdAsync(1))
                .ReturnsAsync(ticket);

            _unitOfWorkMock
                .Setup(u => u.TicketRepository.DeleteAsync(1))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.CommitAsync())
                .Returns(Task.CompletedTask);

            var result = await _service.DeleteAsync(1);

            result.Should().BeTrue();

            _unitOfWorkMock.Verify(u => u.TicketRepository.DeleteAsync(1), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenTicketDoesNotExist()
        {
            _unitOfWorkMock
                .Setup(u => u.TicketRepository.GetByIdAsync(1))
                .ReturnsAsync((Ticket?)null);

            var result = await _service.DeleteAsync(1);

            result.Should().BeFalse();

            _unitOfWorkMock.Verify(u => u.TicketRepository.DeleteAsync(It.IsAny<int>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }
    }
}
