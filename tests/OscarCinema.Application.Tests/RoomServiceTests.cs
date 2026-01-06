using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable;
using MockQueryable.Moq;
using Moq;
using OscarCinema.Application.DTOs.Pagination;
using OscarCinema.Application.DTOs.Room;
using OscarCinema.Application.DTOs.Seat;
using OscarCinema.Application.Services;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Interfaces;

namespace OscarCinema.Application.Tests
{
    public class RoomServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<RoomService>> _loggerMock;
        private readonly RoomService _service;

        public RoomServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<RoomService>>();

            _service = new RoomService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnDto_WhenRoomExists()
        {
            var room = new Room(1, "Sala IMAX");
            var dto = new RoomResponse { Id = 1, Number = 1, Name = "Sala IMAX", Seats = new List<SeatResponse>() };

            _unitOfWorkMock
                .Setup(u => u.RoomRepository.GetByIdAsync(1))
                .ReturnsAsync(room);

            _mapperMock
                .Setup(m => m.Map<RoomResponse>(room))
                .Returns(dto);

            var result = await _service.GetByIdAsync(1);

            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.Name.Should().Be("Sala IMAX");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenRoomDoesNotExist()
        {
            _unitOfWorkMock
                .Setup(u => u.RoomRepository.GetByIdAsync(1))
                .ReturnsAsync((Room?)null);

            var result = await _service.GetByIdAsync(1);

            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateRoom_AndReturnDto()
        {
            var createDto = new CreateRoom
            {
                Number = 1,
                Name = "Sala 1",
            };

            var responseDto = new RoomResponse
            {
                Id = 10,
                Number = 1,
                Name = "Sala 1",
                Seats = new List<SeatResponse>()
            };

            _mapperMock
                .Setup(m => m.Map<RoomResponse>(It.IsAny<Room>()))
                .Returns(responseDto);

            _unitOfWorkMock
                .Setup(u => u.RoomRepository.AddAsync(It.IsAny<Room>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.CommitAsync())
                .Returns(Task.CompletedTask);

            var result = await _service.CreateAsync(createDto);

            result.Should().NotBeNull();
            result.Id.Should().Be(10);
            result.Number.Should().Be(1);

            _unitOfWorkMock.Verify(u => u.RoomRepository.AddAsync(It.IsAny<Room>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenRoomExists()
        {
            var room = new Room(1, "Sala 1");

            _unitOfWorkMock
                .Setup(u => u.RoomRepository.GetByIdAsync(1))
                .ReturnsAsync(room);

            _unitOfWorkMock
                .Setup(u => u.RoomRepository.DeleteAsync(1))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.CommitAsync())
                .Returns(Task.CompletedTask);

            var result = await _service.DeleteAsync(1);

            result.Should().BeTrue();

            _unitOfWorkMock.Verify(u => u.RoomRepository.DeleteAsync(1), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenRoomDoesNotExist()
        {
            _unitOfWorkMock
                .Setup(u => u.RoomRepository.GetByIdAsync(1))
                .ReturnsAsync((Room?)null);

            var result = await _service.DeleteAsync(1);

            result.Should().BeFalse();

            _unitOfWorkMock.Verify(u => u.RoomRepository.DeleteAsync(It.IsAny<int>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateRoom_AndReturnDto()
        {
            var existing = new Room(1, "Sala Antiga");

            var updateDto = new UpdateRoom
            {
                Number = 2,
                Name = "Sala Reformada",
                Seats = new List<SeatResponse>()
            };

            _unitOfWorkMock
                .Setup(u => u.RoomRepository.GetByIdAsync(1))
                .ReturnsAsync(existing);

            _unitOfWorkMock
                .Setup(u => u.RoomRepository.UpdateAsync(existing))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.CommitAsync())
                .Returns(Task.CompletedTask);

            _mapperMock
                .Setup(m => m.Map<RoomResponse>(existing))
                .Returns(new RoomResponse
                {
                    Id = 1,
                    Number = 2,
                    Name = "Sala Reformada",
                    Seats = new List<SeatResponse>()
                });

            var result = await _service.UpdateAsync(1, updateDto);

            result.Should().NotBeNull();
            result!.Name.Should().Be("Sala Reformada");
            result.Number.Should().Be(2);

            existing.Name.Should().Be("Sala Reformada");

            _unitOfWorkMock.Verify(u => u.RoomRepository.UpdateAsync(existing), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPaginatedRooms()
        {
            var rooms = new List<Room>
            {
                new Room(1, "Sala A"),
                new Room(2, "Sala B")
            };

            var responses = new List<RoomResponse>
            {
                new RoomResponse { Id = 1, Name = "Sala A", Number = 1, Seats = new List<SeatResponse>() },
                new RoomResponse { Id = 2, Name = "Sala B", Number = 2, Seats = new List<SeatResponse>() }
            };

            var queryable = rooms.BuildMock();

            _unitOfWorkMock
                .Setup(u => u.RoomRepository.GetAllQueryable())
                .Returns(queryable);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<RoomResponse>>(It.IsAny<IEnumerable<Room>>()))
                .Returns(responses);

            var result = await _service.GetAllAsync(new PaginationQuery { PageNumber = 1, PageSize = 10 });

            result.Data.Should().HaveCount(2);
            result.TotalItems.Should().Be(2);
            result.TotalPages.Should().Be(1);
        }
    }
}
