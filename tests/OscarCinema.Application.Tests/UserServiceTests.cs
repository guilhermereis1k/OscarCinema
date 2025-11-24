using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using MockQueryable;
using MockQueryable.Moq;
using Moq;
using OscarCinema.Application.DTOs.Pagination;
using OscarCinema.Application.DTOs.Session;
using OscarCinema.Application.DTOs.Ticket;
using OscarCinema.Application.DTOs.TicketSeat;
using OscarCinema.Application.DTOs.User;
using OscarCinema.Application.Interfaces;
using OscarCinema.Application.Services;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Entities.Pricing;
using OscarCinema.Domain.Enums.Ticket;
using OscarCinema.Domain.Enums.User;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Domain.Validation;
using Xunit;

namespace OscarCinema.Application.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<UserService>> _loggerMock;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<UserService>>();

            _service = new UserService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnDto_WhenUserExists()
        {
            var user = new User(
                applicationUserId: 100,
                name: "Test User",
                documentNumber: "08202715008",
                email: "test@email.com",
                role: UserRole.USER
            );

            var dto = new UserResponse
            {
                Id = 1,
                Name = "Test User",
                Email = "test@email.com",
                Role = UserRole.USER
            };

            _unitOfWorkMock.Setup(u => u.UserRepository.GetByIdAsync(1))
                .ReturnsAsync(user);

            _mapperMock.Setup(m => m.Map<UserResponse>(user))
                .Returns(dto);

            var result = await _service.GetByIdAsync(1);

            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.Name.Should().Be("Test User");
            result.Email.Should().Be("test@email.com");
            result.Role.Should().Be(UserRole.USER);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            _unitOfWorkMock.Setup(u => u.UserRepository.GetByIdAsync(1))
                .ReturnsAsync((User?)null);

            var result = await _service.GetByIdAsync(1);

            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateUser_AndReturnDto_WhenUserExists()
        {
            var existing = new User(
                applicationUserId: 101,
                name: "Old Name",
                documentNumber: "07992448057",
                email: "old@email.com",
                role: UserRole.USER
            );

            var updateDto = new UpdateUser
            {
                Name = "New Name",
                Email = "new@email.com",
                Role = UserRole.ADMIN
            };

            _unitOfWorkMock.Setup(u => u.UserRepository.GetByIdAsync(1))
                .ReturnsAsync(existing);
            _unitOfWorkMock.Setup(u => u.UserRepository.UpdateAsync(existing))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CommitAsync())
                .Returns(Task.CompletedTask);

            _mapperMock.Setup(m => m.Map<UserResponse>(existing))
                .Returns(new UserResponse
                {
                    Id = 1,
                    Name = "New Name",
                    Email = "new@email.com",
                    Role = UserRole.ADMIN
                });

            var result = await _service.UpdateAsync(1, updateDto);

            result.Should().NotBeNull();
            result!.Name.Should().Be("New Name");
            result.Email.Should().Be("new@email.com");
            result.Role.Should().Be(UserRole.ADMIN);

            _unitOfWorkMock.Verify(u => u.UserRepository.UpdateAsync(existing), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            var updateDto = new UpdateUser
            {
                Name = "New Name",
                Email = "new@email.com",
                Role = UserRole.ADMIN
            };

            _unitOfWorkMock.Setup(u => u.UserRepository.GetByIdAsync(1))
                .ReturnsAsync((User?)null);

            var result = await _service.UpdateAsync(1, updateDto);

            result.Should().BeNull();
            _unitOfWorkMock.Verify(u => u.UserRepository.UpdateAsync(It.IsAny<User>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenUserExists()
        {
            var user = new User(
                applicationUserId: 102,
                name: "Test User",
                documentNumber: "93008519008",
                email: "test@email.com",
                role: UserRole.USER
            );

            _unitOfWorkMock.Setup(u => u.UserRepository.GetByIdAsync(1))
                .ReturnsAsync(user);
            _unitOfWorkMock.Setup(u => u.UserRepository.DeleteAsync(1))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CommitAsync())
                .Returns(Task.CompletedTask);

            var result = await _service.DeleteAsync(1);

            result.Should().BeTrue();
            _unitOfWorkMock.Verify(u => u.UserRepository.DeleteAsync(1), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            _unitOfWorkMock.Setup(u => u.UserRepository.GetByIdAsync(1))
                .ReturnsAsync((User?)null);

            var result = await _service.DeleteAsync(1);

            result.Should().BeFalse();
            _unitOfWorkMock.Verify(u => u.UserRepository.DeleteAsync(It.IsAny<int>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPaginatedUsers()
        {
            var users = new List<User>
            {
                new User(applicationUserId: 200, name: "User 1", documentNumber: "52923050061", email: "user1@email.com", role: UserRole.ADMIN),
                new User(applicationUserId: 201, name: "User 2", documentNumber: "09954133046", email: "user2@email.com", role: UserRole.ADMIN)
            };

            var responses = new List<UserResponse>
            {
                new UserResponse { Id = 1, Name = "User 1", Email = "user1@email.com", Role = UserRole.ADMIN },
                new UserResponse { Id = 2, Name = "User 2", Email = "user2@email.com", Role = UserRole.ADMIN }
            };

            var queryable = users.BuildMock();

            _unitOfWorkMock
                .Setup(u => u.UserRepository.GetAllQueryable())
                .Returns(queryable);

            _mapperMock.Setup(m => m.Map<IEnumerable<UserResponse>>(It.IsAny<IEnumerable<User>>()))
                .Returns(responses);

            var result = await _service.GetAllAsync(new PaginationQuery { PageNumber = 1, PageSize = 10 });

            result.Data.Should().HaveCount(2);
            result.TotalItems.Should().Be(2);
            result.TotalPages.Should().Be(1);
        }
    }
}


