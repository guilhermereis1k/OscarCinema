using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OscarCinema.Application.DTOs.Pagination;
using OscarCinema.Application.DTOs.Ticket;
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
        private readonly TicketService _service;

        public TicketServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<TicketService>>();

            _service = new TicketService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _loggerMock.Object
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
                    new SeatSelection { SeatId = 1, Type = TicketType.Full, Price = 25m }
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



            _unitOfWorkMock
                .Setup(u => u.SessionRepository.GetDetailedAsync(1))
                .ReturnsAsync(session);

            _unitOfWorkMock
                .Setup(u => u.TicketRepository.AddAsync(It.IsAny<Ticket>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.SessionRepository.UpdateAsync(session))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.CommitAsync())
                .Returns(Task.CompletedTask);

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
