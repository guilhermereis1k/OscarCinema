using AutoMapper;
using OscarCinema.Application.DTOs.Seat;
using OscarCinema.Application.Interfaces;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OscarCinema.Application.Services
{
    public class SeatService : ISeatService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SeatService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<SeatResponseDTO> CreateAsync(CreateSeatDTO dto)
        {
            var entity = _mapper.Map<Seat>(dto);
            await _unitOfWork.SeatRepository.AddAsync(entity);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<SeatResponseDTO>(entity);
        }

        public async Task<SeatResponseDTO?> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.SeatRepository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<SeatResponseDTO>(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.SeatRepository.GetByIdAsync(id);
            if (entity == null) return false;

            await _unitOfWork.SeatRepository.DeleteAsync(id);
            await _unitOfWork.CommitAsync();

            return true;
        }
        public async Task<SeatResponseDTO?> GetByRowAndNumberAsync(char row, int number)
        {
            var entity = await _unitOfWork.SeatRepository.GetByRowAndNumberAsync(row, number);
            if (entity == null)
                return null;

            return _mapper.Map<SeatResponseDTO>(entity);
        }

        public async Task<IEnumerable<SeatResponseDTO>?> GetSeatsByRoomIdAsync(int roomId)
        {
            var entity = await _unitOfWork.SeatRepository.GetSeatsByRoomIdAsync(roomId);
            return entity == null ? null : _mapper.Map<IEnumerable<SeatResponseDTO>>(entity);
        }

        public async Task<SeatResponseDTO?> OccupySeatAsync(int id)
        {
            var entity = await _unitOfWork.SeatRepository.GetByIdAsync(id);
            if (entity == null)
                return null;

            entity.OccupySeat(id);

            await _unitOfWork.SeatRepository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<SeatResponseDTO>(entity);
        }

        public async Task<SeatResponseDTO?> FreeSeatAsync(int id)
        {
            var entity = await _unitOfWork.SeatRepository.GetByIdAsync(id);
            if (entity == null)
                return null;

            entity.FreeSeat(id);

            await _unitOfWork.SeatRepository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<SeatResponseDTO>(entity);
        }
    }
}
