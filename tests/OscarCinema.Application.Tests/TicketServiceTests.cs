using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable;
using MockQueryable.Moq;
using Moq;
using OscarCinema.Application.DTOs.Pagination;
using OscarCinema.Application.DTOs.Ticket;
using OscarCinema.Application.DTOs.TicketSeat;
using OscarCinema.Application.Interfaces;
using OscarCinema.Application.Services;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Entities.Pricing;
using OscarCinema.Domain.Enums.Movie;
using OscarCinema.Domain.Enums.Ticket;
using OscarCinema.Domain.Enums.User;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Domain.Validation;
using System.Diagnostics;
using System.Xml.Linq;
using Xunit;

namespace OscarCinema.Application.Tests
{
    public class TicketServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ITicketSeatService> _ticketSeatServiceMock;
        private readonly Mock<IPricingService> _pricingServiceMock;
        private readonly Mock<ILogger<TicketService>> _loggerMock;
        private readonly TicketService _service;

        public TicketServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _ticketSeatServiceMock = new Mock<ITicketSeatService>();
            _pricingServiceMock = new Mock<IPricingService>();
            _loggerMock = new Mock<ILogger<TicketService>>();

            _service = new TicketService(
                _unitOfWorkMock.Object,
                _ticketSeatServiceMock.Object,
                _pricingServiceMock.Object,
                _mapperMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnDto_WhenTicketExists()
        {
            var ticket = new Ticket(1, 1, 1, 1, PaymentMethod.CreditCard, PaymentStatus.Pending, false);
            var ticketSeatsDto = new List<TicketSeatResponse>
            {
                new TicketSeatResponse { Id = 1, TicketId = 1, SeatId = 1 }
            };
            var ticketDto = new TicketResponse
            {
                Id = 1,
                UserId = 1,
                SessionId = 1,
                TicketSeats = ticketSeatsDto
            };

            _unitOfWorkMock.Setup(u => u.TicketRepository.GetByIdAsync(1)).ReturnsAsync(ticket);
            _ticketSeatServiceMock.Setup(s => s.GetByTicketIdAsync(It.IsAny<int>())).ReturnsAsync(ticketSeatsDto);
            _mapperMock.Setup(m => m.Map<TicketResponse>(ticket)).Returns(ticketDto);

            var result = await _service.GetByIdAsync(1);

            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.UserId.Should().Be(1);
            result.TicketSeats.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenTicketDoesNotExist()
        {
            _unitOfWorkMock.Setup(u => u.TicketRepository.GetByIdAsync(1)).ReturnsAsync((Ticket?)null);

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
            new SeatSelection { SeatId = 1, Type = TicketType.Full }
        }
            };

            var session = new Session(1, 1, 1, DateTime.Now.AddHours(2), TimeSpan.FromMinutes(120), TimeSpan.FromMinutes(15));
            var user = new User(1, "Test User", "44328452878", "test@email.com", UserRole.ADMIN);
            var seat = new Seat(1, 'A', 1, 1);

            var ticket = new Ticket(1, 1, 1, 1, PaymentMethod.CreditCard, PaymentStatus.Pending, false);

            _unitOfWorkMock.Setup(u => u.SessionRepository.GetByIdAsync(1)).ReturnsAsync(session);
            _unitOfWorkMock.Setup(u => u.UserRepository.GetByIdAsync(1)).ReturnsAsync(user);
            _unitOfWorkMock.Setup(u => u.SeatRepository.GetByIdAsync(1)).ReturnsAsync(seat);
            _unitOfWorkMock.Setup(u => u.TicketRepository.AddAsync(It.IsAny<Ticket>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

            _mapperMock.Setup(m => m.Map<Ticket>(createDto)).Returns(ticket);
            _mapperMock.Setup(m => m.Map<TicketResponse>(It.IsAny<Ticket>())).Returns(new TicketResponse { Id = 1 });

            _pricingServiceMock.Setup(x => x.CalculateSeatPrice(It.IsAny<ExhibitionType>(), It.IsAny<SeatType>())).Returns(25.0m);
            _pricingServiceMock.Setup(x => x.ApplyTicketType(It.IsAny<decimal>(), It.IsAny<TicketType>())).Returns(25.0m);

            _ticketSeatServiceMock.Setup(x => x.CreateAsync(It.IsAny<CreateTicketSeat>())).ReturnsAsync(new TicketSeatResponse());

            var result = await _service.CreateAsync(createDto);

            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            _unitOfWorkMock.Verify(u => u.TicketRepository.AddAsync(It.IsAny<Ticket>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenTicketExists()
        {
            var ticket = new Ticket(1, 1, 1, 1, PaymentMethod.CreditCard, PaymentStatus.Pending, false);

            _unitOfWorkMock.Setup(u => u.TicketRepository.GetByIdAsync(1)).ReturnsAsync(ticket);
            _unitOfWorkMock.Setup(u => u.TicketRepository.DeleteAsync(1)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

            var result = await _service.DeleteAsync(1);

            result.Should().BeTrue();
            _unitOfWorkMock.Verify(u => u.TicketRepository.DeleteAsync(1), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenTicketDoesNotExist()
        {
            _unitOfWorkMock.Setup(u => u.TicketRepository.GetByIdAsync(1)).ReturnsAsync((Ticket?)null);

            var result = await _service.DeleteAsync(1);

            result.Should().BeFalse();
            _unitOfWorkMock.Verify(u => u.TicketRepository.DeleteAsync(It.IsAny<int>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }
    }
}