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
        public async Task<SeatResponseDTO?> GetByRowAndNumberAsync(GetSeatByRowAndNumberDTO dto)
        {
            var entity = await _unitOfWork.SeatRepository.GetByRowAndNumberAsync(dto.Row, dto.Number);
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
            var seat = await _unitOfWork.SeatRepository.GetByIdAsync(id);
            if (seat == null) return null;

            seat.OccupySeat(id);
            await _unitOfWork.SeatRepository.UpdateAsync(seat);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<SeatResponseDTO>(seat);
        }

        public async Task<SeatResponseDTO?> FreeSeatAsync(int id)
        {
            var seat = await _unitOfWork.SeatRepository.GetByIdAsync(id);
            if (seat == null) return null;

            seat.FreeSeat(id);
            await _unitOfWork.SeatRepository.UpdateAsync(seat);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<SeatResponseDTO>(seat);
        }
    }
}
