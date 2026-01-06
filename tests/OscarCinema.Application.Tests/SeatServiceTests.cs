using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OscarCinema.Application.DTOs.Seat;
using OscarCinema.Application.Services;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Entities.Pricing;
using OscarCinema.Domain.Interfaces;
using Xunit;

namespace OscarCinema.Application.Tests
{
    public class SeatServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<ILogger<SeatService>> _loggerMock = new();
        private readonly SeatService _service;

        public SeatServiceTests()
        {
            _service = new SeatService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateSeat_AndReturnDto()
        {
            var createDto = new CreateSeat
            {
                Row = 'A',
                Number = 1,
                RoomId = 1,
                SeatTypeId = 1
            };

            var room = new Room(1, "Sala 1");

            var seatType = new SeatType(
                "Normal",
                "Assento padrão da sala",
                25m
            );

            var seatEntity = new Seat(1, 'A', 1, 1);

            var responseDto = new SeatResponse
            {
                Id = 10,
                Row = 'A',
                Number = 1,
                RoomId = 1,
                SeatTypeId = 1
            };

            _unitOfWorkMock
                .Setup(u => u.RoomRepository.GetByIdAsync(1))
                .ReturnsAsync(room);

            _unitOfWorkMock
                .Setup(u => u.SeatTypeRepository.GetByIdAsync(1))
                .ReturnsAsync(seatType);

            _mapperMock
                .Setup(m => m.Map<Seat>(createDto))
                .Returns(seatEntity);

            _mapperMock
                .Setup(m => m.Map<SeatResponse>(It.IsAny<Seat>()))
                .Returns(responseDto);

            _unitOfWorkMock
                .Setup(u => u.SeatRepository.AddAsync(It.IsAny<Seat>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.CommitAsync())
                .Returns(Task.CompletedTask);

            var result = await _service.CreateAsync(createDto);

            result.Should().NotBeNull();
            result.Id.Should().Be(10);
            result.Row.Should().Be('A');
            result.Number.Should().Be(1);

            _unitOfWorkMock.Verify(
                u => u.SeatRepository.AddAsync(It.IsAny<Seat>()),
                Times.Once
            );

            _unitOfWorkMock.Verify(
                u => u.CommitAsync(),
                Times.Once
            );
        }
    }
}
