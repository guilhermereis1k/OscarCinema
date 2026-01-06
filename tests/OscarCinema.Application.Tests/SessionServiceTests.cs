using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using OscarCinema.Application.DTOs.Pagination;
using OscarCinema.Application.DTOs.Session;
using OscarCinema.Application.Services;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Entities.Pricing;
using OscarCinema.Domain.Interfaces;
using Xunit;

namespace OscarCinema.Application.Tests
{
    public class SessionServiceTests
    {
        private readonly Mock<IUnitOfWork> _uow;
        private readonly Mock<IMapper> _mapper;
        private readonly SessionService _service;

        public SessionServiceTests()
        {
            _uow = new Mock<IUnitOfWork>();
            _mapper = new Mock<IMapper>();

            var logger = new Mock<ILogger<SessionService>>();

            _service = new SessionService(
                _uow.Object,
                _mapper.Object,
                logger.Object
            );
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsSession_WhenExists()
        {
            var session = new Session(
                movieId: 1,
                roomId: 1,
                exhibitionTypeId: 1,
                startTime: DateTime.Now.AddHours(2),
                durationMinutes: 120
            );

            _uow.Setup(u => u.SessionRepository.GetByIdAsync(1))
                .ReturnsAsync(session);

            var result = await _service.GetByIdAsync(1);

            result.Should().NotBeNull();
            result!.MovieId.Should().Be(1);
            result.RoomId.Should().Be(1);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            _uow.Setup(u => u.SessionRepository.GetByIdAsync(1))
                .ReturnsAsync((Session?)null);

            var result = await _service.GetByIdAsync(1);

            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_CreatesSession()
        {
            _uow.Setup(u => u.MovieRepository.GetByIdAsync(1))
                .ReturnsAsync(Mock.Of<Movie>());

            _uow.Setup(u => u.RoomRepository.GetByIdAsync(1))
                .ReturnsAsync(Mock.Of<Room>());

            _uow.Setup(u => u.ExhibitionTypeRepository.GetByIdAsync(1))
                .ReturnsAsync(Mock.Of<ExhibitionType>());

            _uow.Setup(u => u.SessionRepository.HasTimeConflictAsync(
                    1,
                    It.IsAny<DateTime>(),
                    120,
                    null))
                .ReturnsAsync(false);

            _uow.Setup(u => u.SessionRepository.AddAsync(It.IsAny<Session>()))
                .Returns(Task.CompletedTask);

            _uow.Setup(u => u.CommitAsync())
                .Returns(Task.CompletedTask);

            var result = await _service.CreateAsync(
                movieId: 1,
                roomId: 1,
                exhibitionTypeId: 1,
                startTime: DateTime.Now.AddHours(2),
                durationMinutes: 120
            );

            result.Should().NotBeNull();
            result.MovieId.Should().Be(1);

            _uow.Verify(u => u.SessionRepository.AddAsync(It.IsAny<Session>()), Times.Once);
            _uow.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_DeletesSession()
        {
            _uow.Setup(u => u.SessionRepository.DeleteAsync(1))
                .Returns(Task.CompletedTask);

            _uow.Setup(u => u.CommitAsync())
                .Returns(Task.CompletedTask);

            await _service.DeleteAsync(1);

            _uow.Verify(u => u.SessionRepository.DeleteAsync(1), Times.Once);
            _uow.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsPaginatedResult()
        {

            var sessions = new List<Session>
            {
                new Session(1, 1, 1, DateTime.Now.AddHours(2), 120),
                new Session(2, 1, 1, DateTime.Now.AddHours(4), 120)
            };

            var queryable = sessions.AsQueryable();

            _uow.Setup(u => u.SessionRepository.GetAllQueryable())
                .Returns(queryable);

            _mapper.Setup(m => m.Map<IEnumerable<SessionResponse>>(sessions))
                .Returns(new List<SessionResponse>
                {
            new() { Id = 1, MovieId = 1 },
            new() { Id = 2, MovieId = 2 }
                });

            var result = new PaginationResult<SessionResponse>
            {
                CurrentPage = 1,
                PageSize = 10,
                TotalItems = 2,
                TotalPages = 1,
                Data = _mapper.Object.Map<IEnumerable<SessionResponse>>(sessions)
            };

            result.TotalItems.Should().Be(2);
            result.Data.Should().HaveCount(2);
        }
    }
}
