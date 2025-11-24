using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using OscarCinema.Application.DTOs.ExhibitionType;
using OscarCinema.Application.DTOs.Pagination;
using OscarCinema.Application.DTOs.Seat;
using OscarCinema.Application.DTOs.SeatType;
using OscarCinema.Application.Services;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Entities.Pricing;
using OscarCinema.Domain.Interfaces;

namespace OscarCinema.Application.Tests
{
    public class SeatServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<SeatService>> _loggerMock;
        private readonly SeatService _service;

        public SeatServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<SeatService>>();

            _service = new SeatService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnDto_WhenSeatExists()
        {
            var seat = new Seat(1, 'A', 1, 1);
            var dto = new SeatResponse { Id = 1, Row = 'A', Number = 1, RoomId = 1, SeatTypeId = 1 };

            _unitOfWorkMock
                .Setup(u => u.SeatRepository.GetByIdAsync(1))
                .ReturnsAsync(seat);

            _mapperMock
                .Setup(m => m.Map<SeatResponse>(seat))
                .Returns(dto);

            var result = await _service.GetByIdAsync(1);

            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.Row.Should().Be('A');
            result.Number.Should().Be(1);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenSeatDoesNotExist()
        {
            _unitOfWorkMock
                .Setup(u => u.SeatRepository.GetByIdAsync(1))
                .ReturnsAsync((Seat?)null);

            var result = await _service.GetByIdAsync(1);

            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateSeat_AndReturnDto()
        {
            var createDto = new CreateSeat { Row = 'A', Number = 1, RoomId = 1, SeatTypeId = 1 };
            var responseDto = new SeatResponse { Id = 10, Row = 'A', Number = 1, RoomId = 1, SeatTypeId = 1 };

            var seatEntity = new Seat(1, 'A', 1, 1);

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

            _unitOfWorkMock.Verify(u => u.SeatRepository.AddAsync(It.IsAny<Seat>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenSeatExists()
        {
            var seat = new Seat(1, 'A', 1, 1);

            _unitOfWorkMock
                .Setup(u => u.SeatRepository.GetByIdAsync(1))
                .ReturnsAsync(seat);

            _unitOfWorkMock
                .Setup(u => u.SeatRepository.DeleteAsync(1))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.CommitAsync())
                .Returns(Task.CompletedTask);

            var result = await _service.DeleteAsync(1);

            result.Should().BeTrue();

            _unitOfWorkMock.Verify(u => u.SeatRepository.DeleteAsync(1), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenSeatDoesNotExist()
        {
            _unitOfWorkMock
                .Setup(u => u.SeatRepository.GetByIdAsync(1))
                .ReturnsAsync((Seat?)null);

            var result = await _service.DeleteAsync(1);

            result.Should().BeFalse();

            _unitOfWorkMock.Verify(u => u.SeatRepository.DeleteAsync(It.IsAny<int>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task GetByRowAndNumberAsync_ShouldReturnDto_WhenSeatExists()
        {
            var getDto = new GetSeatByRowAndNumber { Row = 'A', Number = 1 };
            var seat = new Seat(1, 'A', 1, 1);
            var dto = new SeatResponse { Id = 1, Row = 'A', Number = 1, RoomId = 1, SeatTypeId = 1 };

            _unitOfWorkMock
                .Setup(u => u.SeatRepository.GetByRowAndNumberAsync('A', 1))
                .ReturnsAsync(seat);

            _mapperMock
                .Setup(m => m.Map<SeatResponse>(seat))
                .Returns(dto);

            var result = await _service.GetByRowAndNumberAsync(getDto);

            result.Should().NotBeNull();
            result!.Row.Should().Be('A');
            result.Number.Should().Be(1);
        }

        [Fact]
        public async Task GetByRowAndNumberAsync_ShouldReturnNull_WhenSeatDoesNotExist()
        {
            var getDto = new GetSeatByRowAndNumber { Row = 'A', Number = 1 };

            _unitOfWorkMock
                .Setup(u => u.SeatRepository.GetByRowAndNumberAsync('A', 1))
                .ReturnsAsync((Seat?)null);

            var result = await _service.GetByRowAndNumberAsync(getDto);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetSeatsByRoomIdAsync_ShouldReturnSeats_WhenRoomHasSeats()
        {
            var seats = new List<Seat>
            {
                new Seat(1, 'A', 1, 1),
                new Seat(1, 'A', 2, 1)
            };

            var responses = new List<SeatResponse>
            {
                new SeatResponse { Id = 1, Row = 'A', Number = 1, RoomId = 1 },
                new SeatResponse { Id = 2, Row = 'A', Number = 2, RoomId = 1 }
            };

            _unitOfWorkMock
                .Setup(u => u.SeatRepository.GetSeatsByRoomIdAsync(1))
                .ReturnsAsync(seats);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<SeatResponse>>(seats))
                .Returns(responses);

            var result = await _service.GetSeatsByRoomIdAsync(1);

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetSeatsByRoomIdAsync_ShouldReturnNull_WhenRoomHasNoSeats()
        {
            _unitOfWorkMock
                .Setup(u => u.SeatRepository.GetSeatsByRoomIdAsync(1))
                .ReturnsAsync((IEnumerable<Seat>?)null);

            var result = await _service.GetSeatsByRoomIdAsync(1);

            result.Should().BeNull();
        }

        [Fact]
        public async Task OccupySeatAsync_ShouldOccupySeat_AndReturnDto_WhenSeatExistsAndIsFree()
        {
            var seat = new Seat(1, 'A', 1, 1);
            var occupiedSeatResponse = new SeatResponse { Id = 1, Row = 'A', Number = 1, RoomId = 1, SeatTypeId = 1, IsOccupied = true };

            _unitOfWorkMock
                .Setup(u => u.SeatRepository.GetByIdAsync(1))
                .ReturnsAsync(seat);

            _unitOfWorkMock
                .Setup(u => u.SeatRepository.UpdateAsync(seat))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.CommitAsync())
                .Returns(Task.CompletedTask);

            _mapperMock
                .Setup(m => m.Map<SeatResponse>(seat))
                .Returns(occupiedSeatResponse);

            var result = await _service.OccupySeatAsync(1);

            result.Should().NotBeNull();
            result!.IsOccupied.Should().BeTrue();

            _unitOfWorkMock.Verify(u => u.SeatRepository.UpdateAsync(seat), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task OccupySeatAsync_ShouldReturnNull_WhenSeatDoesNotExist()
        {
            _unitOfWorkMock
                .Setup(u => u.SeatRepository.GetByIdAsync(1))
                .ReturnsAsync((Seat?)null);

            var result = await _service.OccupySeatAsync(1);

            result.Should().BeNull();

            _unitOfWorkMock.Verify(u => u.SeatRepository.UpdateAsync(It.IsAny<Seat>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task FreeSeatAsync_ShouldFreeSeat_AndReturnDto_WhenSeatExistsAndIsOccupied()
        {
            var seat = new Seat(1, true, 'A', 1, 1);
            var freedSeatResponse = new SeatResponse { Id = 1, Row = 'A', Number = 1, RoomId = 1, SeatTypeId = 1, IsOccupied = false };

            _unitOfWorkMock
                .Setup(u => u.SeatRepository.GetByIdAsync(1))
                .ReturnsAsync(seat);

            _unitOfWorkMock
                .Setup(u => u.SeatRepository.UpdateAsync(seat))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.CommitAsync())
                .Returns(Task.CompletedTask);

            _mapperMock
                .Setup(m => m.Map<SeatResponse>(seat))
                .Returns(freedSeatResponse);

            var result = await _service.FreeSeatAsync(1);

            result.Should().NotBeNull();
            result!.IsOccupied.Should().BeFalse();

            _unitOfWorkMock.Verify(u => u.SeatRepository.UpdateAsync(seat), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task FreeSeatAsync_ShouldReturnNull_WhenSeatDoesNotExist()
        {
            _unitOfWorkMock
                .Setup(u => u.SeatRepository.GetByIdAsync(1))
                .ReturnsAsync((Seat?)null);

            var result = await _service.FreeSeatAsync(1);

            result.Should().BeNull();

            _unitOfWorkMock.Verify(u => u.SeatRepository.UpdateAsync(It.IsAny<Seat>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }
    }
}