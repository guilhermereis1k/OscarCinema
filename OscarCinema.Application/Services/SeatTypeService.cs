using AutoMapper;
using Microsoft.Extensions.Logging;
using OscarCinema.Application.DTOs;
using OscarCinema.Application.DTOs.SeatType;
using OscarCinema.Application.Interfaces;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Entities.Pricing;
using OscarCinema.Domain.Interfaces;
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
        private readonly ILogger<SeatTypeService> _logger;

        public SeatTypeService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<SeatTypeService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<SeatTypeResponseDTO>> GetAllAsync()
        {
            _logger.LogDebug("Getting all seat types");

            var entities = await _unitOfWork.SeatTypeRepository.GetAllAsync();
            var seatTypes = _mapper.Map<IEnumerable<SeatTypeResponseDTO>>(entities);

            _logger.LogDebug("Retrieved {Count} seat types", seatTypes.Count());
            return seatTypes;
        }

        public async Task<SeatTypeResponseDTO?> GetByIdAsync(int id)
        {
            _logger.LogDebug("Getting seat type by ID: {Id}", id);

            var entity = await _unitOfWork.SeatTypeRepository.GetByIdAsync(id);

            if (entity == null)
            {
                _logger.LogWarning("Seat type not found: {Id}", id);
                return null;
            }

            _logger.LogDebug("Seat type found: {Name} (ID: {Id})", entity.Name, id);
            return _mapper.Map<SeatTypeResponseDTO>(entity);
        }

        public async Task<SeatTypeResponseDTO> CreateAsync(CreateSeatTypeDTO dto)
        {
            _logger.LogInformation("Creating new seat type: {Name} with price {Price}", dto.Name, dto.Price);

            var entity = _mapper.Map<SeatType>(dto);
            await _unitOfWork.SeatTypeRepository.AddAsync(entity);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Seat type created successfully: {Name} (ID: {Id})", entity.Name, entity.Id);
            return _mapper.Map<SeatTypeResponseDTO>(entity);
        }

        public async Task<SeatTypeResponseDTO> UpdateAsync(int id, UpdateSeatTypeDTO dto)
        {
            _logger.LogInformation("Updating seat type ID: {Id}", id);

            var entity = await _unitOfWork.SeatTypeRepository.GetByIdAsync(id);
            if (entity == null)
            {
                _logger.LogError("Seat type not found for update: {Id}", id);
                throw new KeyNotFoundException($"SeatType with ID {id} not found.");
            }

            entity.Update(dto.Name, dto.Description, dto.IsActive);
            entity.UpdatePrice(dto.Price);

            await _unitOfWork.SeatTypeRepository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Seat type updated successfully: {Name} (ID: {Id})", entity.Name, id);
            return _mapper.Map<SeatTypeResponseDTO>(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting seat type: {Id}", id);

            var entity = await _unitOfWork.SeatTypeRepository.GetByIdAsync(id);
            if (entity == null)
            {
                _logger.LogWarning("Seat type not found for deletion: {Id}", id);
                return false;
            }

            await _unitOfWork.SeatTypeRepository.DeleteAsync(id);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Seat type deleted successfully: {Id}", id);
            return true;
        }
    }
}