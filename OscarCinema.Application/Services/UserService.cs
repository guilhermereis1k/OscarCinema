using AutoMapper;
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

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserResponseDTO> CreateAsync(CreateUserDTO request)
        {
            var user = _mapper.Map<User>(request);
            await _unitOfWork.UserRepository.AddAsync(user);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<UserResponseDTO>(user);
        }

        public async Task<UserResponseDTO?> UpdateAsync(int id, UpdateUserDTO request)
        {
            var existentUser = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (existentUser == null)
                return null;

            _mapper.Map(request, existentUser);
            await _unitOfWork.UserRepository.UpdateAsync(existentUser);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<UserResponseDTO>(existentUser);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (user == null)
                return false;

            await _unitOfWork.UserRepository.DeleteAsync(id);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<UserResponseDTO?> GetByIdAsync(int id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (user == null)
                return null;

            return _mapper.Map<UserResponseDTO>(user);
        }

        public async Task<IEnumerable<UserResponseDTO>> GetAllAsync()
        {
            var users = await _unitOfWork.UserRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserResponseDTO>>(users ?? Enumerable.Empty<User>());
        }
    }
}
