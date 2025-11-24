using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using OscarCinema.Application.DTOs.ExhibitionType;
using OscarCinema.Application.DTOs.Pagination;
using OscarCinema.Application.DTOs.SeatType;
using OscarCinema.Application.Services;
using OscarCinema.Domain.Entities.Pricing;
using OscarCinema.Domain.Interfaces;

namespace OscarCinema.Application.Tests
{
    public class ExhibitionTypeServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<ExhibitionTypeService>> _loggerMock;
        private readonly ExhibitionTypeService _service;

        public ExhibitionTypeServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<ExhibitionTypeService>>();

            _service = new ExhibitionTypeService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnDto_WhenEntityExists()
        {
            var entity = new ExhibitionType("IMAX", "Big screen", "4K HDR", 50m);
            var dto = new ExhibitionTypeResponse { Id = 1, Name = "IMAX" };

            _unitOfWorkMock
                .Setup(u => u.ExhibitionTypeRepository.GetByIdAsync(1))
                .ReturnsAsync(entity);

            _mapperMock
                .Setup(m => m.Map<ExhibitionTypeResponse>(entity))
                .Returns(dto);

            var result = await _service.GetByIdAsync(1);

            result.Should().NotBeNull();
            result!.Name.Should().Be("IMAX");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenEntityDoesNotExist()
        {
            _unitOfWorkMock
                .Setup(u => u.ExhibitionTypeRepository.GetByIdAsync(1))
                .ReturnsAsync((ExhibitionType?)null);

            var result = await _service.GetByIdAsync(1);

            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnDto_WhenEntityCreated()
        {
            var createDto = new CreateExhibitionType
            {
                Name = "IMAX",
                Description = "Big screen",
                TechnicalSpecs = "4K HDR",
                Price = 50m
            };

            var entity = new ExhibitionType("IMAX", "Big screen", "4K HDR", 50m);
            var resDto = new ExhibitionTypeResponse { Id = 1, Name = "IMAX" };

            _mapperMock
                .Setup(m => m.Map<ExhibitionType>(createDto))
                .Returns(entity);

            _mapperMock
                .Setup(m => m.Map<ExhibitionTypeResponse>(entity))
                .Returns(resDto);

            _unitOfWorkMock
                .Setup(u => u.ExhibitionTypeRepository.AddAsync(entity))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.CommitAsync())
                .Returns(Task.CompletedTask);

            var result = await _service.CreateAsync(createDto);

            result.Should().NotBeNull();
            result.Should().BeOfType<ExhibitionTypeResponse>();
            result.Id.Should().Be(1);
            result.Name.Should().Be("IMAX");

            _unitOfWorkMock.Verify(u => u.ExhibitionTypeRepository.AddAsync(entity), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnDto_WhenEntityUpdated()
        {
            var updateDto = new UpdateExhibitionType
            {

                Name = "FULLHD",
                Description = "Big screen",
                TechnicalSpecs = "4K HDR",
                Price = 50m
            };


            var entity = new ExhibitionType("IMAX", "Big screen", "4K HDR", 50m);

            var expectedDto = new ExhibitionTypeResponse { Id = 1, Name = "FULLHD" };

            _unitOfWorkMock
                .Setup(u => u.ExhibitionTypeRepository.GetByIdAsync(1))
                .ReturnsAsync(entity);

            _unitOfWorkMock
                .Setup(u => u.ExhibitionTypeRepository.UpdateAsync(entity))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.CommitAsync())
                .Returns(Task.CompletedTask);

            _mapperMock
                .Setup(m => m.Map<ExhibitionTypeResponse>(It.IsAny<ExhibitionType>()))
                .Returns<ExhibitionType>(e => new ExhibitionTypeResponse
                {
                    Id = 1,
                    Name = e.Name
                });

            var result = await _service.UpdateAsync(1, updateDto);

            result.Should().NotBeNull();
            result.Should().BeOfType<ExhibitionTypeResponse>();
            result.Id.Should().Be(1);
            result.Name.Should().Be("FULLHD");

            entity.Name.Should().Be("FULLHD");
            entity.Description.Should().Be("Big screen");
            entity.TechnicalSpecs.Should().Be("4K HDR");
            entity.Price.Should().Be(50m);

            _unitOfWorkMock.Verify(u => u.ExhibitionTypeRepository.UpdateAsync(entity), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPaginatedMovies()
        {
            var exhibitionTypes = new List<ExhibitionType>
            {
                new ExhibitionType("Full HD", "Large screen", "HDR", 20m),
                new ExhibitionType("IMAX", "Big screen", "4K HDR", 50m)
            };

            var responses = new List<ExhibitionTypeResponse>
            {
                new ExhibitionTypeResponse { Id = 1, Name = "Full HD", Price = 20m },
                new ExhibitionTypeResponse { Id = 2, Name = "IMAX", Price = 50m }
            };

            var queryable = exhibitionTypes.BuildMock();

            _unitOfWorkMock
                .Setup(u => u.ExhibitionTypeRepository.GetAllQueryable())
                .Returns(queryable);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<ExhibitionTypeResponse>>(It.IsAny<IEnumerable<ExhibitionType>>()))
                .Returns(responses);

            var result = await _service.GetAllAsync(new PaginationQuery { PageNumber = 1, PageSize = 10 });

            result.Data.Should().HaveCount(2);
            result.TotalItems.Should().Be(2);
            result.TotalPages.Should().Be(1);
        }
    }
}