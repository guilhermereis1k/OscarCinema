using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OscarCinema.Application.DTOs.Pagination;
using OscarCinema.Application.DTOs.User;
using OscarCinema.Application.Interfaces;
using OscarCinema.Domain.Common.ValueObjects;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Entities.Pricing;
using OscarCinema.Domain.Enums.User;
using OscarCinema.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        public async Task<UserResponse> CreateAsync(CreateUser request)
        {
            var cpf = new Cpf(request.DocumentNumber);

            var exists = await _unitOfWork.UserRepository
                .FindByDocumentIdAsync(cpf.Number);

            if (exists != null)
                throw new InvalidOperationException("Document already registered.");

            var user = new User(
                request.ApplicationUserId,
                request.Name,
                cpf.Number,
                request.Email,
                request.Role
            );

            await _unitOfWork.UserRepository.AddAsync(user);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<UserResponse>(user);
        }


        public async Task<UserResponse?> UpdateAsync(int id, UpdateUser request)
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
            return _mapper.Map<UserResponse>(existentUser);
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

        public async Task<UserResponse?> GetByIdAsync(int id)
        {
            _logger.LogDebug("Getting user by ID: {UserId}", id);

            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", id);
                return null;
            }

            _logger.LogDebug("User found: {Email} (ID: {UserId})", user.Email, id);
            return _mapper.Map<UserResponse>(user);
        }

        public async Task<PaginationResult<UserResponse>> GetAllAsync(PaginationQuery query)
        {
            _logger.LogDebug("Getting all users with pagination");

            var baseQuery = _unitOfWork.UserRepository.GetAllQueryable();

            var totalItems = await baseQuery.CountAsync();

            var users = await baseQuery
                .OrderBy(r => r.Id)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            var userDtos = _mapper.Map<IEnumerable<UserResponse>>(users);

            _logger.LogDebug("Retrieved {UserCount} users.", userDtos.Count());

            return new PaginationResult<UserResponse>
            {
                CurrentPage = query.PageNumber,
                PageSize = query.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize),
                Data = userDtos
            };
        }

        public async Task<User> GetByApplicationUserIdAsync(int appUserId)
        {
            return await _unitOfWork.UserRepository.GetByApplicationUserIdAsync(appUserId);
        }
    }
}