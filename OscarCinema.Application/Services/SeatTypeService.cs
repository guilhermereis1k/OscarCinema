using AutoMapper;
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
        private readonly IGenericRepository<SeatType> _repository;
        private readonly IMapper _mapper;

        public SeatTypeService(IGenericRepository<SeatType> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SeatTypeResponseDTO>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<SeatTypeResponseDTO>>(entities);
        }

        public async Task<SeatTypeResponseDTO?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<SeatTypeResponseDTO>(entity);
        }

        public async Task<SeatTypeResponseDTO> CreateAsync(CreateSeatTypeDTO dto)
        {
            var entity = _mapper.Map<SeatType>(dto);
            await _repository.AddAsync(entity);
            return _mapper.Map<SeatTypeResponseDTO>(entity);
        }

        public async Task UpdateAsync(int id, CreateSeatTypeDTO dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"SeatType with ID {id} not found.");

            entity.Update(dto.Name, dto.Description, dto.IsActive);
            entity.UpdatePrice(dto.Price);

            await _repository.UpdateAsync(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var seatType = await _repository.GetByIdAsync(id);
            if (seatType == null) return false;

            await _repository.DeleteAsync(id);
            return true;
        }
    }
}
