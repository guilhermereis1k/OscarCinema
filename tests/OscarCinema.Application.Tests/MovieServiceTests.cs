using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using OscarCinema.Application.DTOs.ExhibitionType;
using OscarCinema.Application.DTOs.Movie;
using OscarCinema.Application.DTOs.Pagination;
using OscarCinema.Application.Services;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Entities.Pricing;
using OscarCinema.Domain.Enums.Movie;
using OscarCinema.Domain.Interfaces;

namespace OscarCinema.Application.Tests
{
    public class MovieServiceTests
    {
            private readonly Mock<IUnitOfWork> _unitOfWorkMock;
            private readonly Mock<IMapper> _mapperMock;
            private readonly Mock<ILogger<MovieService>> _loggerMock;
            private readonly MovieService _service;

            public MovieServiceTests()
            {
                _unitOfWorkMock = new Mock<IUnitOfWork>();
                _mapperMock = new Mock<IMapper>();
                _loggerMock = new Mock<ILogger<MovieService>>();

                _service = new MovieService(
                    _unitOfWorkMock.Object,
                    _mapperMock.Object,
                    _loggerMock.Object
                );
            }

            [Fact]
            public async Task GetByIdAsync_ShouldReturnDto_WhenEntityExists()
            {
                var entity = new Movie(
                    "Inception",
                    "A valid description with more than twenty characters",
                    "https://example.com/inception.jpg",
                    120,
                    MovieGenre.Action,
                    AgeRating.Age12
                );

                var dto = new MovieResponse
                {
                    Id = 1,
                    Title = "Inception"
                };

                _unitOfWorkMock
                    .Setup(u => u.MovieRepository.GetByIdAsync(1))
                    .ReturnsAsync(entity);

                _mapperMock
                    .Setup(m => m.Map<MovieResponse>(entity))
                    .Returns(dto);

                var result = await _service.GetByIdAsync(1);

                result.Should().NotBeNull();
                result!.Title.Should().Be("Inception");
            }

            [Fact]
            public async Task GetByIdAsync_ShouldReturnNull_WhenEntityDoesNotExist()
            {
                _unitOfWorkMock
                    .Setup(u => u.MovieRepository.GetByIdAsync(1))
                    .ReturnsAsync((Movie?)null);

                var result = await _service.GetByIdAsync(1);

                result.Should().BeNull();
            }

            [Fact]
            public async Task CreateAsync_ShouldReturnDto_WhenEntityCreated()
            {
                var createDto = new CreateMovie
                {
                    Title = "Inception",
                    Description = "A valid description long enough",
                    ImageUrl = "https://example.com/inception.jpg",
                    Duration = 120,
                    Genre = MovieGenre.Action,
                    AgeRating = AgeRating.Age12
                };

                var entity = new Movie(
                    createDto.Title,
                    createDto.Description,
                    createDto.ImageUrl,
                    createDto.Duration,
                    createDto.Genre,
                    createDto.AgeRating
                );

                var responseDto = new MovieResponse
                {
                    Id = 1,
                    Title = "Inception"
                };

                _mapperMock
                    .Setup(m => m.Map<Movie>(createDto))
                    .Returns(entity);

                _mapperMock
                    .Setup(m => m.Map<MovieResponse>(entity))
                    .Returns(responseDto);

                _unitOfWorkMock
                    .Setup(u => u.MovieRepository.AddAsync(entity))
                    .Returns(Task.CompletedTask);

                _unitOfWorkMock
                    .Setup(u => u.CommitAsync())
                    .Returns(Task.CompletedTask);

                var result = await _service.CreateAsync(createDto);

                result.Should().NotBeNull();
                result.Id.Should().Be(1);
                result.Title.Should().Be("Inception");

                _unitOfWorkMock.Verify(u => u.MovieRepository.AddAsync(entity), Times.Once);
                _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
            }

            [Fact]
            public async Task UpdateAsync_ShouldReturnDto_WhenEntityUpdated()
            {
                var updateDto = new UpdateMovie
                {
                    Title = "Inception 2",
                    Description = "Updated description long enough",
                    ImageUrl = "https://example.com/inception2.jpg",
                    Duration = 140,
                    Genre = MovieGenre.Action,
                    AgeRating = AgeRating.Age12
                };

                var entity = new Movie(
                    "Inception",
                    "A valid description long enough",
                    "https://example.com/inception.jpg",
                    120,
                    MovieGenre.Action,
                    AgeRating.Age12
                );

                _unitOfWorkMock
                    .Setup(u => u.MovieRepository.GetByIdAsync(1))
                    .ReturnsAsync(entity);

                _mapperMock
                    .Setup(m => m.Map(updateDto, entity))
                    .Callback(() =>
                    {
                        entity.Update(
                            updateDto.Title,
                            updateDto.Description,
                            updateDto.ImageUrl,
                            updateDto.Duration,
                            updateDto.Genre,
                            updateDto.AgeRating
                        );
                    });

                _unitOfWorkMock
                    .Setup(u => u.MovieRepository.UpdateAsync(entity))
                    .Returns(Task.CompletedTask);

                _unitOfWorkMock
                    .Setup(u => u.CommitAsync())
                    .Returns(Task.CompletedTask);

                _mapperMock
                    .Setup(m => m.Map<MovieResponse>(entity))
                    .Returns(new MovieResponse
                    {
                        Id = 1,
                        Title = "Inception 2"
                    });

                var result = await _service.UpdateAsync(1, updateDto);

                result.Should().NotBeNull();
                result!.Title.Should().Be("Inception 2");

                entity.Title.Should().Be("Inception 2");

                _unitOfWorkMock.Verify(u => u.MovieRepository.UpdateAsync(entity), Times.Once);
                _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
            }

            [Fact]
            public async Task GetAllAsync_ShouldReturnPaginatedMovies()
            {
                var movies = new List<Movie>
            {
                new Movie("Inception", "A valid description long enough", "https://example.com/inception.jpg", 120, MovieGenre.Action, AgeRating.Age12),
                new Movie("Inception 2", "A valid description long enough", "https://example.com/inception.jpg", 90, MovieGenre.Action, AgeRating.Age12),
            };

                    var responses = new List<MovieResponse>
            {
                new MovieResponse { Id = 1, Title = "Inception", Duration = 120 },
                new MovieResponse { Id = 2, Title = "Inception 2", Duration = 90 }
            };

                var queryable = movies.BuildMock();

                _unitOfWorkMock
                    .Setup(u => u.MovieRepository.GetAllQueryable())
                    .Returns(queryable);

                _mapperMock
                    .Setup(m => m.Map<IEnumerable<MovieResponse>>(It.IsAny<IEnumerable<Movie>>()))
                    .Returns(responses);

                var result = await _service.GetAllAsync(new PaginationQuery { PageNumber = 1, PageSize = 10 });

                result.Data.Should().HaveCount(2);
                result.TotalItems.Should().Be(2);
                result.TotalPages.Should().Be(1);
            }
    }
}