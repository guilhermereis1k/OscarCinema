using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OscarCinema.Application.DTOs.TicketSeat;
using OscarCinema.Application.Services;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Enums.Ticket;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Domain.Validation;
using Xunit;

namespace OscarCinema.Application.Tests
{
    public class TicketSeatServiceTests
    {
        private readonly Mock<IUnitOfWork> _uow = new();
        private readonly Mock<IMapper> _mapper = new();
        private readonly TicketSeatService _service;

        public TicketSeatServiceTests()
        {
            var logger = new Mock<ILogger<TicketSeatService>>();

            _service = new TicketSeatService(
                _uow.Object,
                _mapper.Object,
                logger.Object
            );
        }

        [Fact]
        public async Task UpdatePriceAsync_UpdatesPrice_WhenTicketAndSeatExist()
        {
            var ticket = new Ticket(1, 1, 1, 1, PaymentMethod.CreditCard);
            var seat = new TicketSeat(ticket.Id, 2, TicketType.Full, 25m);
            ticket.AddTicketSeat(seat);

            _uow.Setup(u => u.TicketRepository.GetByIdAsync(ticket.Id))
                .ReturnsAsync(ticket);

            _uow.Setup(u => u.TicketRepository.UpdateAsync(ticket))
                .Returns(Task.CompletedTask);

            _uow.Setup(u => u.CommitAsync())
                .Returns(Task.CompletedTask);

            _mapper.Setup(m => m.Map<TicketSeatResponse>(seat))
                .Returns(() => new TicketSeatResponse
                {
                    SeatId = seat.SeatId,
                    Type = seat.Type,
                    Price = seat.Price
                });

            var result = await _service.UpdatePriceAsync(ticket.Id, seat.SeatId, 35m);

            result.Should().NotBeNull();
            result.Price.Should().Be(35m);

            _uow.Verify(u => u.TicketRepository.UpdateAsync(ticket), Times.Once);
            _uow.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdatePriceAsync_Throws_WhenTicketNotFound()
        {
            _uow.Setup(u => u.TicketRepository.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Ticket?)null);

            Func<Task> act = () =>
                _service.UpdatePriceAsync(1, 1, 50m);

            await act.Should().ThrowAsync<DomainExceptionValidation>()
                .WithMessage("Ticket not found.");
        }

        [Fact]
        public async Task UpdatePriceAsync_Throws_WhenTicketSeatNotFound()
        {
            var ticket = new Ticket(1, 1, 1, 1, PaymentMethod.CreditCard);

            _uow.Setup(u => u.TicketRepository.GetByIdAsync(ticket.Id))
                .ReturnsAsync(ticket);

            Func<Task> act = () =>
                _service.UpdatePriceAsync(ticket.Id, 999, 50m);

            await act.Should().ThrowAsync<DomainExceptionValidation>()
                .WithMessage("TicketSeat not found.");
        }
    }
}
