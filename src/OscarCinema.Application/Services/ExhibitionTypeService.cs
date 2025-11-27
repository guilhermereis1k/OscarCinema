using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OscarCinema.Application.DTOs.ExhibitionType;
using OscarCinema.Application.DTOs.Pagination;
using OscarCinema.Application.DTOs.Room;
using OscarCinema.Application.Interfaces;
using OscarCinema.Domain.Entities.Pricing;
using OscarCinema.Domain.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace OscarCinema.Application.Services
{
    public class ExhibitionTypeService : IExhibitionTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ExhibitionTypeService> _logger;

        public ExhibitionTypeService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ExhibitionTypeService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginationResult<ExhibitionTypeResponse>> GetAllAsync(PaginationQuery query)
        {
            _logger.LogDebug("Getting all exhibition types with pagination");

            var baseQuery = _unitOfWork.ExhibitionTypeRepository.GetAllQueryable();

            var totalItems = await baseQuery.CountAsync();

            var exhibitionTypes = await baseQuery
                .OrderBy(r => r.Id)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            var exhibitionTypeDtos = _mapper.Map<IEnumerable<ExhibitionTypeResponse>>(exhibitionTypes);

            _logger.LogDebug("Retrieved {ExhibitionTypesCount} exhibition types.", exhibitionTypeDtos.Count());

            return new PaginationResult<ExhibitionTypeResponse>
            {
                CurrentPage = query.PageNumber,
                PageSize = query.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize),
                Data = exhibitionTypeDtos
            };
        }

        public async Task<ExhibitionTypeResponse?> GetByIdAsync(int id)
        {
            _logger.LogDebug("Getting exhibition type by ID: {Id}", id);

            var entity = await _unitOfWork.ExhibitionTypeRepository.GetByIdAsync(id);

            if (entity == null)
            {
                _logger.LogWarning("Exhibition type not found: {Id}", id);
                return null;
            }

            _logger.LogDebug("Exhibition type found: {Name} (ID: {Id})", entity.Name, id);
            return _mapper.Map<ExhibitionTypeResponse>(entity);
        }

        public async Task<ExhibitionTypeResponse> CreateAsync(CreateExhibitionType dto)
        {
            _logger.LogInformation("Creating new exhibition type: {Name}", dto.Name);

            var entity = _mapper.Map<ExhibitionType>(dto);
            await _unitOfWork.ExhibitionTypeRepository.AddAsync(entity);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Exhibition type created successfully: {Name} (ID: {Id})", entity.Name, entity.Id);
            return _mapper.Map<ExhibitionTypeResponse>(entity);
        }

        public async Task<ExhibitionTypeResponse> UpdateAsync(int id, UpdateExhibitionType dto)
        {
            _logger.LogInformation("Updating exhibition type ID: {Id}", id);

            var entity = await _unitOfWork.ExhibitionTypeRepository.GetByIdAsync(id);
            if (entity == null)
            {
                _logger.LogError("Exhibition type not found for update: {Id}", id);
                throw new KeyNotFoundException($"ExhibitionType with ID {id} not found.");
            }

            entity.Update(dto.Name, dto.Description, dto.TechnicalSpecs);
            entity.UpdatePrice(dto.Price);

            await _unitOfWork.ExhibitionTypeRepository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Exhibition type updated successfully: {Name} (ID: {Id})", entity.Name, id);
            return _mapper.Map<ExhibitionTypeResponse>(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting exhibition type: {Id}", id);

            var entity = await _unitOfWork.ExhibitionTypeRepository.GetByIdAsync(id);
            if (entity == null)
            {
                _logger.LogWarning("Exhibition type not found for deletion: {Id}", id);
                return false;
            }

            await _unitOfWork.ExhibitionTypeRepository.DeleteAsync(id);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Exhibition type deleted successfully: {Id}", id);
            return true;
        }
    }
}