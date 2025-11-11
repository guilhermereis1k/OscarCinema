using AutoMapper;
using Microsoft.Extensions.Logging;
using OscarCinema.Application.DTOs.User;
using OscarCinema.Application.Interfaces;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Entities.Pricing;
using OscarCinema.Domain.Enums.User;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UserService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UserResponseDTO> CreateAsync(CreateUserDTO request)
        {
            _logger.LogInformation("Creating new user: {Email}", request.Email);

            var user = _mapper.Map<User>(request);
            await _unitOfWork.UserRepository.AddAsync(user);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("User created successfully: {Email} (ID: {UserId})", request.Email, user.Id);
            return _mapper.Map<UserResponseDTO>(user);
        }

        public async Task<UserResponseDTO?> UpdateAsync(int id, UpdateUserDTO request)
        {
            _logger.LogInformation("Updating user ID: {UserId}", id);

            var existentUser = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (existentUser == null)
            {
                _logger.LogWarning("User not found for update: {UserId}", id);
                return null;
            }

            _mapper.Map(request, existentUser);
            await _unitOfWork.UserRepository.UpdateAsync(existentUser);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("User updated successfully: {UserId}", id);
            return _mapper.Map<UserResponseDTO>(existentUser);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting user: {UserId}", id);

            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User not found for deletion: {UserId}", id);
                return false;
            }

            await _unitOfWork.UserRepository.DeleteAsync(id);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("User deleted successfully: {UserId}", id);
            return true;
        }

        public async Task<UserResponseDTO?> GetByIdAsync(int id)
        {
            _logger.LogDebug("Getting user by ID: {UserId}", id);

            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", id);
                return null;
            }

            _logger.LogDebug("User found: {Email} (ID: {UserId})", user.Email, id);
            return _mapper.Map<UserResponseDTO>(user);
        }

        public async Task<IEnumerable<UserResponseDTO>> GetAllAsync()
        {
            _logger.LogDebug("Getting all users");

            var users = await _unitOfWork.UserRepository.GetAllAsync();
            var userDtos = _mapper.Map<IEnumerable<UserResponseDTO>>(users ?? Enumerable.Empty<User>());

            _logger.LogDebug("Retrieved {Count} users", userDtos.Count());
            return userDtos;
        }
    }
}