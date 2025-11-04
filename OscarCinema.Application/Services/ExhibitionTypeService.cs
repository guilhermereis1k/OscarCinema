using AutoMapper;
using OscarCinema.Application.DTOs.ExhibitionType;
using OscarCinema.Application.DTOs.Movie;
using OscarCinema.Application.Interfaces;
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
    public class ExhibitionTypeService : IExhibitionTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ExhibitionTypeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ExhibitionTypeResponseDTO>> GetAllAsync()
        {
            var entities = await _unitOfWork.ExhibitionTypeRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ExhibitionTypeResponseDTO>>(entities);
        }

        public async Task<ExhibitionTypeResponseDTO?> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.ExhibitionTypeRepository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<ExhibitionTypeResponseDTO>(entity);
        }

        public async Task<ExhibitionTypeResponseDTO> CreateAsync(CreateExhibitionTypeDTO dto)
        {
            var entity = _mapper.Map<ExhibitionType>(dto);
            await _unitOfWork.ExhibitionTypeRepository.AddAsync(entity);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<ExhibitionTypeResponseDTO>(entity);
        }

        public async Task<ExhibitionTypeResponseDTO> UpdateAsync(int id, UpdateExhibitionTypeDTO dto)
        {
            var entity = await _unitOfWork.ExhibitionTypeRepository.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"ExhibitionType with ID {id} not found.");

            entity.Update(dto.Name, dto.Description, dto.TechnicalSpecs, dto.IsActive);
            entity.UpdatePrice(dto.Price);

            await _unitOfWork.ExhibitionTypeRepository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<ExhibitionTypeResponseDTO>(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.ExhibitionTypeRepository.GetByIdAsync(id);
            if (entity == null) return false;

            await _unitOfWork.ExhibitionTypeRepository.DeleteAsync(id);
            await _unitOfWork.CommitAsync();

            return true;
        }
    }
}
