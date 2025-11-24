using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using OscarCinema.Application.DTOs.ExhibitionType;
using OscarCinema.Application.DTOs.Movie;
using OscarCinema.Application.DTOs.Pagination;
using OscarCinema.Application.DTOs.Session;
using OscarCinema.Application.Services;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Entities.Pricing;
using OscarCinema.Domain.Enums.Movie;
using OscarCinema.Domain.Interfaces;

namespace OscarCinema.Application.Tests
{
    public class SessionServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<SessionService>> _loggerMock;
        private readonly SessionService _service;

        public SessionServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<SessionService>>();

            _service = new SessionService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnDto_WhenSessionExists()
        {
            var session = new Session(1, 1, 1, DateTime.Now.AddHours(1), TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(10));
            var dto = new SessionResponse { Id = 1, MovieId = 1, RoomId = 1, ExhibitionTypeId = 1 };

            _unitOfWorkMock
                .Setup(u => u.SessionRepository.GetByIdAsync(1))
                .ReturnsAsync(session);

            _mapperMock
                .Setup(m => m.Map<SessionResponse>(session))
                .Returns(dto);

            var result = await _service.GetByIdAsync(1);

            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.MovieId.Should().Be(1);
            result.RoomId.Should().Be(1);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenSessionDoesNotExist()
        {
            _unitOfWorkMock
                .Setup(u => u.SessionRepository.GetByIdAsync(1))
                .ReturnsAsync((Session?)null);

            var result = await _service.GetByIdAsync(1);

            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateSession_AndReturnDto()
        {
            var createDto = new CreateSession
            {
                MovieId = 1,
                RoomId = 1,
                ExhibitionTypeId = 1,
                StartTime = DateTime.Now.AddHours(1)
            };

            var responseDto = new SessionResponse
            {
                Id = 10,
                MovieId = 1,
                RoomId = 1,
                ExhibitionTypeId = 1
            };

            _mapperMock
                .Setup(m => m.Map<SessionResponse>(It.IsAny<Session>()))
                .Returns(responseDto);

            _unitOfWorkMock
                .Setup(u => u.SessionRepository.AddAsync(It.IsAny<Session>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.CommitAsync())
                .Returns(Task.CompletedTask);

            var result = await _service.CreateAsync(createDto);

            result.Should().NotBeNull();
            result.Id.Should().Be(10);
            result.MovieId.Should().Be(1);
            result.RoomId.Should().Be(1);

            _unitOfWorkMock.Verify(u => u.SessionRepository.AddAsync(It.IsAny<Session>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenSessionExists()
        {
            var session = new Session(1, 1, 1, DateTime.Now.AddHours(1), TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(10));

            _unitOfWorkMock
                .Setup(u => u.SessionRepository.GetByIdAsync(1))
                .ReturnsAsync(session);

            _unitOfWorkMock
                .Setup(u => u.SessionRepository.DeleteAsync(1))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.CommitAsync())
                .Returns(Task.CompletedTask);

            var result = await _service.DeleteAsync(1);

            result.Should().BeTrue();

            _unitOfWorkMock.Verify(u => u.SessionRepository.DeleteAsync(1), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenSessionDoesNotExist()
        {
            _unitOfWorkMock
                .Setup(u => u.SessionRepository.GetByIdAsync(1))
                .ReturnsAsync((Session?)null);

            var result = await _service.DeleteAsync(1);

            result.Should().BeFalse();

            _unitOfWorkMock.Verify(u => u.SessionRepository.DeleteAsync(It.IsAny<int>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateSession_AndReturnDto_WhenSessionExists()
        {
            var existing = new Session(1, 1, 1, DateTime.Now.AddHours(2), TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(10));

            var updateDto = new UpdateSession
            {
                MovieId = 2,
                RoomId = 2,
                ExhibitionTypeId = 2,
                StartTime = DateTime.Now.AddHours(3)
            };

            _unitOfWorkMock
                .Setup(u => u.SessionRepository.GetByIdAsync(1))
                .ReturnsAsync(existing);

            _unitOfWorkMock
                .Setup(u => u.SessionRepository.UpdateAsync(existing))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.CommitAsync())
                .Returns(Task.CompletedTask);

            _mapperMock
                .Setup(m => m.Map<SessionResponse>(existing))
                .Returns(new SessionResponse
                {
                    Id = 1,
                    MovieId = 2,
                    RoomId = 2,
                    ExhibitionTypeId = 2
                });

            var result = await _service.UpdateAsync(1, updateDto);

            result.Should().NotBeNull();
            result!.MovieId.Should().Be(2);
            result.RoomId.Should().Be(2);
            result.ExhibitionTypeId.Should().Be(2);

            _unitOfWorkMock.Verify(u => u.SessionRepository.UpdateAsync(existing), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnNull_WhenSessionDoesNotExist()
        {
            var updateDto = new UpdateSession
            {
                MovieId = 2,
                RoomId = 2,
                ExhibitionTypeId = 2,
                StartTime = DateTime.Now.AddHours(3)
            };

            _unitOfWorkMock
                .Setup(u => u.SessionRepository.GetByIdAsync(1))
                .ReturnsAsync((Session?)null);

            var result = await _service.UpdateAsync(1, updateDto);

            result.Should().BeNull();

            _unitOfWorkMock.Verify(u => u.SessionRepository.UpdateAsync(It.IsAny<Session>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPaginatedSessions()
        {
            var sessions = new List<Session>
            {
                new Session(1, 1, 1, DateTime.Now.AddHours(1), TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(10)),
                new Session(2, 1, 1, DateTime.Now.AddHours(3), TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(10))
            };

            var responses = new List<SessionResponse>
            {
                new SessionResponse { Id = 1, MovieId = 1, RoomId = 1 },
                new SessionResponse { Id = 2, MovieId = 2, RoomId = 1 }
            };

            var queryable = sessions.BuildMock();

            _unitOfWorkMock
                .Setup(u => u.SessionRepository.GetAllQueryable())
                .Returns(queryable);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<SessionResponse>>(It.IsAny<IEnumerable<Session>>()))
                .Returns(responses);

            var result = await _service.GetAllAsync(new PaginationQuery { PageNumber = 1, PageSize = 10 });

            result.Data.Should().HaveCount(2);
            result.TotalItems.Should().Be(2);
            result.TotalPages.Should().Be(1);
        }

        [Fact]
        public async Task GetAllByMovieIdAsync_ShouldReturnPaginatedSessions()
        {
            var sessions = new List<Session>
            {
                new Session(1, 1, 1, DateTime.Now.AddHours(1), TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(10)),
                new Session(1, 1, 1, DateTime.Now.AddHours(3), TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(10))
            };

            var responses = new List<SessionResponse>
            {
                new SessionResponse { Id = 1, MovieId = 1, RoomId = 1 },
                new SessionResponse { Id = 2, MovieId = 1, RoomId = 1 }
            };

            var queryable = sessions.BuildMock();

            _unitOfWorkMock
                .Setup(u => u.SessionRepository.GetAllQueryable())
                .Returns(queryable);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<SessionResponse>>(It.IsAny<IEnumerable<Session>>()))
                .Returns(responses);

            var result = await _service.GetAllByMovieIdAsync(new PaginationQuery { PageNumber = 1, PageSize = 10 }, 1);

            result.Data.Should().HaveCount(2);
            result.TotalItems.Should().Be(2);
            result.TotalPages.Should().Be(1);
        }
    }
}