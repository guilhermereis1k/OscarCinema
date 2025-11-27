using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using OscarCinema.Application.DTOs;
using OscarCinema.Application.DTOs.ExhibitionType;
using OscarCinema.Application.DTOs.Movie;
using OscarCinema.Application.DTOs.Pagination;
using OscarCinema.Application.DTOs.SeatType;
using OscarCinema.Application.Services;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Entities.Pricing;
using OscarCinema.Domain.Enums.Movie;
using OscarCinema.Domain.Interfaces;

namespace OscarCinema.Application.Tests
{
    public class SeatTypeServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<SeatTypeService>> _loggerMock;
        private readonly SeatTypeService _service;

        public SeatTypeServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<SeatTypeService>>();

            _service = new SeatTypeService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnDto_WhenEntityExists()
        {
            var entity = new SeatType(
                "VIP",
                "Large seat with premium comfort",
                50m
            );

            var dto = new SeatTypeResponse
            {
                Id = 1,
                Name = "VIP",
                Description = "Large seat with premium comfort",
                Price = 50m
            };

            _unitOfWorkMock
                .Setup(u => u.SeatTypeRepository.GetByIdAsync(1))
                .ReturnsAsync(entity);

            _mapperMock
                .Setup(m => m.Map<SeatTypeResponse>(entity))
                .Returns(dto);

            var result = await _service.GetByIdAsync(1);

            result.Should().NotBeNull();
            result!.Name.Should().Be("VIP");
            result.Price.Should().Be(50m);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenEntityDoesNotExist()
        {
            _unitOfWorkMock
                .Setup(u => u.SeatTypeRepository.GetByIdAsync(1))
                .ReturnsAsync((SeatType?)null);

            var result = await _service.GetByIdAsync(1);

            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnDto_WhenEntityCreated()
        {
            var createDto = new CreateSeatType
            {
                Name = "VIP",
                Description = "Large seat",
                Price = 40m
            };

            var entity = new SeatType(
                createDto.Name,
                createDto.Description,
                createDto.Price
            );

            var responseDto = new SeatTypeResponse
            {
                Id = 1,
                Name = "VIP",
                Description = "Large seat",
                Price = 40m
            };

            _mapperMock
                .Setup(m => m.Map<SeatType>(createDto))
                .Returns(entity);

            _mapperMock
                .Setup(m => m.Map<SeatTypeResponse>(entity))
                .Returns(responseDto);

            _unitOfWorkMock
                .Setup(u => u.SeatTypeRepository.AddAsync(entity))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.CommitAsync())
                .Returns(Task.CompletedTask);

            var result = await _service.CreateAsync(createDto);

            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("VIP");

            _unitOfWorkMock.Verify(u => u.SeatTypeRepository.AddAsync(entity), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnDto_WhenEntityUpdated()
        {
            var updateDto = new UpdateSeatType
            {
                Name = "Premium",
                Description = "Updated premium seat",
                Price = 60m,
            };

            var entity = new SeatType(
                "VIP",
                "Large seat",
                40m
            );

            _unitOfWorkMock
                .Setup(u => u.SeatTypeRepository.GetByIdAsync(1))
                .ReturnsAsync(entity);

            _unitOfWorkMock
                .Setup(u => u.SeatTypeRepository.UpdateAsync(entity))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.CommitAsync())
                .Returns(Task.CompletedTask);

            var responseDto = new SeatTypeResponse
            {
                Id = 1,
                Name = "Premium",
                Description = "Updated premium seat",
                Price = 60m,
            };

            _mapperMock
                .Setup(m => m.Map<SeatTypeResponse>(entity))
                .Returns(responseDto);

            var result = await _service.UpdateAsync(1, updateDto);

            result.Should().NotBeNull();
            result.Name.Should().Be("Premium");
            result.Price.Should().Be(60m);

            entity.Name.Should().Be("Premium");
            entity.Description.Should().Be("Updated premium seat");
            entity.Price.Should().Be(60m);

            _unitOfWorkMock.Verify(u => u.SeatTypeRepository.UpdateAsync(entity), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPaginatedMovies()
        {
            var seatTypes = new List<SeatType>
            {
                new SeatType("VIP", "Large seat", 40m),
                new SeatType("Mini", "Short seat", 30m),
            };

            var responses = new List<SeatTypeResponse>
            {
                new SeatTypeResponse { Id = 1, Name = "VIP", Price = 40m },
                new SeatTypeResponse { Id = 2, Name = "Mini", Price = 30m }
            };

            var queryable = seatTypes.BuildMock();

            _unitOfWorkMock
                .Setup(u => u.SeatTypeRepository.GetAllQueryable())
                .Returns(queryable);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<SeatTypeResponse>>(It.IsAny<IEnumerable<SeatType>>()))
                .Returns(responses);

            var result = await _service.GetAllAsync(new PaginationQuery { PageNumber = 1, PageSize = 10 });

            result.Data.Should().HaveCount(2);
            result.TotalItems.Should().Be(2);
            result.TotalPages.Should().Be(1);
        }
    }
}