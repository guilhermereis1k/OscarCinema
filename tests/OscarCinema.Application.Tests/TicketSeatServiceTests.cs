using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using OscarCinema.Application.DTOs.ExhibitionType;
using OscarCinema.Application.DTOs.Pagination;
using OscarCinema.Application.DTOs.SeatType;
using OscarCinema.Application.DTOs.TicketSeat;
using OscarCinema.Application.Services;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Entities.Pricing;
using OscarCinema.Domain.Enums.Ticket;
using OscarCinema.Domain.Interfaces;

namespace OscarCinema.Application.Tests
{
    public class TicketSeatServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<ILogger<TicketSeatService>> _loggerMock = new();
        private readonly TicketSeatService _service;

        public TicketSeatServiceTests()
        {
            _service = new TicketSeatService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateTicketSeat_AndReturnDto()
        {
            var createDto = new CreateTicketSeat
            {
                TicketId = 1,
                SeatId = 1,
                Type = TicketType.Full,
                Price = 25.50m
            };

            var responseDto = new TicketSeatResponse
            {
                Id = 10,
                TicketId = 1,
                SeatId = 1,
                Type = TicketType.Full,
                Price = 25.50m
            };

            _mapperMock
                .Setup(m => m.Map<TicketSeatResponse>(It.IsAny<TicketSeat>()))
                .Returns(responseDto);

            _unitOfWorkMock
                .Setup(u => u.TicketSeatRepository.AddAsync(It.IsAny<TicketSeat>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.CommitAsync())
                .Returns(Task.CompletedTask);

            var result = await _service.CreateAsync(createDto);

            result.Should().NotBeNull();
            result.Id.Should().Be(10);
            result.TicketId.Should().Be(1);
            result.SeatId.Should().Be(1);
            result.Type.Should().Be(TicketType.Full);
            result.Price.Should().Be(25.50m);

            _unitOfWorkMock.Verify(u => u.TicketSeatRepository.AddAsync(It.IsAny<TicketSeat>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateMultipleAsync_ShouldCreateMultipleTicketSeats_AndReturnDtos()
        {
            var createDtos = new List<CreateTicketSeat>
            {
                new CreateTicketSeat { TicketId = 1, SeatId = 1, Type = TicketType.Half, Price = 25.50m },
                new CreateTicketSeat { TicketId = 1, SeatId = 2, Type = TicketType.Full, Price = 15.00m },
                new CreateTicketSeat { TicketId = 1, SeatId = 3, Type = TicketType.Full, Price = 20.00m }
            };

            var responseDtos = new List<TicketSeatResponse>
            {
                new TicketSeatResponse { Id = 1, TicketId = 1, SeatId = 1, Type = TicketType.Half, Price = 25.50m },
                new TicketSeatResponse { Id = 2, TicketId = 1, SeatId = 2, Type = TicketType.Full, Price = 15.00m },
                new TicketSeatResponse { Id = 3, TicketId = 1, SeatId = 3, Type = TicketType.Full, Price = 20.00m }
            };

            _mapperMock
                .Setup(m => m.Map<IEnumerable<TicketSeatResponse>>(It.IsAny<IEnumerable<TicketSeat>>()))
                .Returns(responseDtos);

            _unitOfWorkMock
                .Setup(u => u.TicketSeatRepository.CreateRangeAsync(It.IsAny<IEnumerable<TicketSeat>>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.CommitAsync())
                .Returns(Task.CompletedTask);

            var result = await _service.CreateMultipleAsync(createDtos);

            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.First().TicketId.Should().Be(1);
            result.First().Type.Should().Be(TicketType.Half);
            result.Last().Type.Should().Be(TicketType.Full);

            _unitOfWorkMock.Verify(u => u.TicketSeatRepository.CreateRangeAsync(It.IsAny<IEnumerable<TicketSeat>>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateMultipleAsync_ShouldHandleEmptyList()
        {
            var emptyDtos = new List<CreateTicketSeat>();
            var emptyResponse = new List<TicketSeatResponse>();

            _mapperMock
                .Setup(m => m.Map<IEnumerable<TicketSeatResponse>>(It.IsAny<IEnumerable<TicketSeat>>()))
                .Returns(emptyResponse);

            _unitOfWorkMock
                .Setup(u => u.TicketSeatRepository.CreateRangeAsync(It.IsAny<IEnumerable<TicketSeat>>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.CommitAsync())
                .Returns(Task.CompletedTask);

            var result = await _service.CreateMultipleAsync(emptyDtos);

            result.Should().NotBeNull();
            result.Should().BeEmpty();

            _unitOfWorkMock.Verify(u => u.TicketSeatRepository.CreateRangeAsync(It.IsAny<IEnumerable<TicketSeat>>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnTicketSeat_WhenExists()
        {
            var entity = new TicketSeat(1, 2, TicketType.Full, 50m);

            _unitOfWorkMock.Setup(u => u.TicketSeatRepository.GetByIdAsync(5))
                    .ReturnsAsync(entity);

            _mapperMock.Setup(m => m.Map<TicketSeatResponse>(entity))
                       .Returns(new TicketSeatResponse { Id = 5, Price = 50 });

            var result = await _service.GetByIdAsync(5);

            result.Should().NotBeNull();
            result!.Price.Should().Be(50m);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            _unitOfWorkMock.Setup(u => u.TicketSeatRepository.GetByIdAsync(5))
                    .ReturnsAsync((TicketSeat)null);

            var result = await _service.GetByIdAsync(5);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByTicketIdAsync_ShouldReturnMappedList()
        {
            var list = new List<TicketSeat>
        {
            new TicketSeat(1, 10, TicketType.Half, 20),
            new TicketSeat(1, 11, TicketType.Full, 40)
        };

            _unitOfWorkMock.Setup(u => u.TicketSeatRepository.GetByTicketIdAsync(1))
                    .ReturnsAsync(list);

            _mapperMock.Setup(m => m.Map<IEnumerable<TicketSeatResponse>>(list))
                       .Returns(new List<TicketSeatResponse>
                       {
                       new TicketSeatResponse { Id = 1, Price = 20 },
                       new TicketSeatResponse { Id = 2, Price = 40 }
                       });

            var result = await _service.GetByTicketIdAsync(1);

            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetBySeatIdAsync_ShouldReturnMappedList()
        {
            var list = new List<TicketSeat>
        {
            new TicketSeat(10, 5, TicketType.Full, 30)
        };

            _unitOfWorkMock.Setup(u => u.TicketSeatRepository.GetBySeatIdAsync(5))
                    .ReturnsAsync(list);

            _mapperMock.Setup(m => m.Map<IEnumerable<TicketSeatResponse>>(list))
                       .Returns(new List<TicketSeatResponse>
                       {
                       new TicketSeatResponse { Id = 1, Price = 30 }
                       });

            var result = await _service.GetBySeatIdAsync(5);

            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task UpdatePriceAsync_ShouldUpdate_WhenExists()
        {
            var entity = new TicketSeat(1, 2, TicketType.Full, 25m);

            _unitOfWorkMock.Setup(u => u.TicketSeatRepository.GetByIdAsync(3))
                    .ReturnsAsync(entity);

            _mapperMock.Setup(m => m.Map<TicketSeatResponse>(entity))
                       .Returns(new TicketSeatResponse { Id = 3, Price = 35 });

            var result = await _service.UpdatePriceAsync(3, 35m);

            result.Should().NotBeNull();
            result!.Price.Should().Be(35m);

            _unitOfWorkMock.Verify(u => u.TicketSeatRepository.UpdateAsync(entity), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdatePriceAsync_ShouldReturnNull_WhenNotFound()
        {
            _unitOfWorkMock.Setup(u => u.TicketSeatRepository.GetByIdAsync(3))
                    .ReturnsAsync((TicketSeat)null);

            var result = await _service.UpdatePriceAsync(3, 50m);

            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_ShouldDelete_WhenExists()
        {
            var entity = new TicketSeat(1, 2, TicketType.Full, 20m);

            _unitOfWorkMock.Setup(u => u.TicketSeatRepository.GetByIdAsync(7))
                    .ReturnsAsync(entity);

            var result = await _service.DeleteAsync(7);

            result.Should().BeTrue();
            _unitOfWorkMock.Verify(u => u.TicketSeatRepository.DeleteAsync(7), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenNotFound()
        {
            _unitOfWorkMock.Setup(u => u.TicketSeatRepository.GetByIdAsync(7))
                    .ReturnsAsync((TicketSeat)null);

            var result = await _service.DeleteAsync(7);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task CalculateTicketTotalAsync_ShouldSumCorrectly()
        {
            var list = new List<TicketSeat>
        {
            new TicketSeat(1, 2, TicketType.Full, 20),
            new TicketSeat(1, 3, TicketType.Full, 30)
        };

            _unitOfWorkMock.Setup(u => u.TicketSeatRepository.GetByTicketIdAsync(1))
                    .ReturnsAsync(list);

            var total = await _service.CalculateTicketTotalAsync(1);

            total.Should().Be(50m);
        }
    }
}