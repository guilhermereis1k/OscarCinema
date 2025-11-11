using AutoMapper;
using Microsoft.Extensions.Logging;
using OscarCinema.Application.DTOs.ExhibitionType;
using OscarCinema.Application.Interfaces;
using OscarCinema.Domain.Entities.Pricing;
using OscarCinema.Domain.Interfaces;

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

        public async Task<IEnumerable<ExhibitionTypeResponseDTO>> GetAllAsync()
        {
            _logger.LogDebug("Getting all exhibition types");

            var entities = await _unitOfWork.ExhibitionTypeRepository.GetAllAsync();

            _logger.LogDebug("Retrieved {Count} exhibition types", entities.Count());
            return _mapper.Map<IEnumerable<ExhibitionTypeResponseDTO>>(entities);
        }

        public async Task<ExhibitionTypeResponseDTO?> GetByIdAsync(int id)
        {
            _logger.LogDebug("Getting exhibition type by ID: {Id}", id);

            var entity = await _unitOfWork.ExhibitionTypeRepository.GetByIdAsync(id);

            if (entity == null)
            {
                _logger.LogWarning("Exhibition type not found: {Id}", id);
                return null;
            }

            _logger.LogDebug("Exhibition type found: {Name} (ID: {Id})", entity.Name, id);
            return _mapper.Map<ExhibitionTypeResponseDTO>(entity);
        }

        public async Task<ExhibitionTypeResponseDTO> CreateAsync(CreateExhibitionTypeDTO dto)
        {
            _logger.LogInformation("Creating new exhibition type: {Name}", dto.Name);

            var entity = _mapper.Map<ExhibitionType>(dto);
            await _unitOfWork.ExhibitionTypeRepository.AddAsync(entity);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Exhibition type created successfully: {Name} (ID: {Id})", entity.Name, entity.Id);
            return _mapper.Map<ExhibitionTypeResponseDTO>(entity);
        }

        public async Task<ExhibitionTypeResponseDTO> UpdateAsync(int id, UpdateExhibitionTypeDTO dto)
        {
            _logger.LogInformation("Updating exhibition type ID: {Id}", id);

            var entity = await _unitOfWork.ExhibitionTypeRepository.GetByIdAsync(id);
            if (entity == null)
            {
                _logger.LogError("Exhibition type not found for update: {Id}", id);
                throw new KeyNotFoundException($"ExhibitionType with ID {id} not found.");
            }

            entity.Update(dto.Name, dto.Description, dto.TechnicalSpecs, dto.IsActive);
            entity.UpdatePrice(dto.Price);

            await _unitOfWork.ExhibitionTypeRepository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Exhibition type updated successfully: {Name} (ID: {Id})", entity.Name, id);
            return _mapper.Map<ExhibitionTypeResponseDTO>(entity);
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