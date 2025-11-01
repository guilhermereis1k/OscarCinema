using AutoMapper;
using OscarCinema.Application.DTOs.ExhibitionType;
using OscarCinema.Application.Interfaces;
using OscarCinema.Domain.Entities.Pricing;
using OscarCinema.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.Services
{
    public class ExhibitionTypeService : IExhibitionTypeService
    {
        private readonly IGenericRepository<ExhibitionType> _repository;
        private readonly IMapper _mapper;

        public ExhibitionTypeService(IGenericRepository<ExhibitionType> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ExhibitionTypeResponseDTO>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<ExhibitionTypeResponseDTO>>(entities);
        }

        public async Task<ExhibitionTypeResponseDTO?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<ExhibitionTypeResponseDTO>(entity);
        }

        public async Task<ExhibitionTypeResponseDTO> CreateAsync(CreateExhibitionTypeDTO dto)
        {
            var entity = _mapper.Map<ExhibitionType>(dto);
            await _repository.AddAsync(entity);
            return _mapper.Map<ExhibitionTypeResponseDTO>(entity);
        }

        public async Task UpdateAsync(int id, CreateExhibitionTypeDTO dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"ExhibitionType with ID {id} not found.");

            entity.Update(dto.Name, dto.Description, dto.TechnicalSpecs, dto.IsActive);
            entity.UpdatePrice(dto.Price);

            await _repository.UpdateAsync(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var exhibitionType = await _repository.GetByIdAsync(id);
            if (exhibitionType == null) return false;

            await _repository.DeleteAsync(id);
            return true
        }
    }
}
