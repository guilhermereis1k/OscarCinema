using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OscarCinema.Application.DTOs;
using OscarCinema.Application.DTOs.Pagination;
using OscarCinema.Application.DTOs.Room;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        public async Task<PaginationResult<SeatTypeResponse>> GetAllAsync(PaginationQuery query)
        {
            _logger.LogDebug("Getting all seat types with pagination");

            var baseQuery = _unitOfWork.SeatTypeRepository.GetAllQueryable();

            var totalItems = await baseQuery.CountAsync();

            var seatTypes = await baseQuery
                .OrderBy(r => r.Id)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            var seatTypesDtos = _mapper.Map<IEnumerable<SeatTypeResponse>>(seatTypes);

            _logger.LogDebug("Retrieved {SeatTypesCount} seat types", seatTypesDtos.Count());

            return new PaginationResult<SeatTypeResponse>
            {
                CurrentPage = query.PageNumber,
                PageSize = query.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize),
                Data = seatTypesDtos
            };
        }

        public async Task<SeatTypeResponse?> GetByIdAsync(int id)
        {
            _logger.LogDebug("Getting seat type by ID: {Id}", id);

            var entity = await _unitOfWork.SeatTypeRepository.GetByIdAsync(id);

            if (entity == null)
            {
                _logger.LogWarning("Seat type not found: {Id}", id);
                return null;
            }

            _logger.LogDebug("Seat type found: {Name} (ID: {Id})", entity.Name, id);
            return _mapper.Map<SeatTypeResponse>(entity);
        }

        public async Task<SeatTypeResponse> CreateAsync(CreateSeatType dto)
        {
            _logger.LogInformation("Creating new seat type: {Name} with price {Price}", dto.Name, dto.Price);

            var entity = _mapper.Map<SeatType>(dto);
            await _unitOfWork.SeatTypeRepository.AddAsync(entity);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Seat type created successfully: {Name} (ID: {Id})", entity.Name, entity.Id);
            return _mapper.Map<SeatTypeResponse>(entity);
        }

        public async Task<SeatTypeResponse> UpdateAsync(int id, UpdateSeatType dto)
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
            return _mapper.Map<SeatTypeResponse>(entity);
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