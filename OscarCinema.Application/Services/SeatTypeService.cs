using AutoMapper;
using OscarCinema.Application.DTOs.SeatType;
using OscarCinema.Application.Interfaces;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Entities.Pricing;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.Services
{
    public class SeatTypeService : ISeatTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SeatTypeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SeatTypeResponseDTO>> GetAllAsync()
        {
            var entities = await _unitOfWork.SeatTypeRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<SeatTypeResponseDTO>>(entities);
        }

        public async Task<SeatTypeResponseDTO?> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.SeatTypeRepository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<SeatTypeResponseDTO>(entity);
        }

        public async Task<SeatTypeResponseDTO> CreateAsync(CreateSeatTypeDTO dto)
        {
            var entity = _mapper.Map<SeatType>(dto);
            await _unitOfWork.SeatTypeRepository.AddAsync(entity);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<SeatTypeResponseDTO>(entity);
        }

        public async Task<SeatTypeResponseDTO> UpdateAsync(int id, CreateSeatTypeDTO dto)
        {
            var entity = await _unitOfWork.SeatTypeRepository.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"SeatType with ID {id} not found.");

            entity.Update(dto.Name, dto.Description, dto.IsActive);
            entity.UpdatePrice(dto.Price);

            await _unitOfWork.SeatTypeRepository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<SeatTypeResponseDTO>(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.SeatTypeRepository.GetByIdAsync(id);
            if (entity == null) return false;

            await _unitOfWork.SeatTypeRepository.DeleteAsync(id);
            await _unitOfWork.CommitAsync();

            return true;
        }
    }
}
