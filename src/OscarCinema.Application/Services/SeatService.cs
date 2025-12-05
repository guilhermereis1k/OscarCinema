using AutoMapper;
using Microsoft.Extensions.Logging;
using OscarCinema.Application.DTOs.Seat;
using OscarCinema.Application.Interfaces;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Domain.Validation;
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
        private readonly ILogger<SeatService> _logger;

        public SeatService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<SeatService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<SeatResponse> CreateAsync(CreateSeat dto)
        {
            _logger.LogInformation("Creating new seat - Row: {Row}, Number: {Number}, Room: {RoomId}",
                dto.Row, dto.Number, dto.RoomId);

            var room = await _unitOfWork.RoomRepository.GetByIdAsync(dto.RoomId);
            if (room == null)
                throw new DomainExceptionValidation("RoomId does not exist.");

            var seatType = await _unitOfWork.SeatTypeRepository.GetByIdAsync(dto.SeatTypeId);
            if (seatType == null)
                throw new DomainExceptionValidation("SeatTypeId does not exist.");

            var entity = _mapper.Map<Seat>(dto);
            await _unitOfWork.SeatRepository.AddAsync(entity);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Seat created successfully: {Row}{Number} (ID: {SeatId})",
                dto.Row, dto.Number, entity.Id);
            return _mapper.Map<SeatResponse>(entity);
        }

        public async Task<SeatResponse?> GetByIdAsync(int id)
        {
            _logger.LogDebug("Getting seat by ID: {SeatId}", id);

            var entity = await _unitOfWork.SeatRepository.GetByIdAsync(id);

            if (entity == null)
            {
                _logger.LogWarning("Seat not found: {SeatId}", id);
                return null;
            }

            _logger.LogDebug("Seat found: {Row}{Number} (ID: {SeatId})", entity.Row, entity.Number, id);
            return _mapper.Map<SeatResponse>(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting seat: {SeatId}", id);

            var entity = await _unitOfWork.SeatRepository.GetByIdAsync(id);
            if (entity == null)
            {
                _logger.LogWarning("Seat not found for deletion: {SeatId}", id);
                return false;
            }

            await _unitOfWork.SeatRepository.DeleteAsync(id);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Seat deleted successfully: {SeatId}", id);
            return true;
        }

        public async Task<SeatResponse?> GetByRowAndNumberAsync(GetSeatByRowAndNumber dto)
        {
            _logger.LogDebug("Getting seat by row and number - Row: {Row}, Number: {Number}",
                dto.Row, dto.Number);

            var entity = await _unitOfWork.SeatRepository.GetByRowAndNumberAsync(dto.Row, dto.Number);

            if (entity == null)
            {
                _logger.LogWarning("Seat not found - Row: {Row}, Number: {Number}", dto.Row, dto.Number);
                return null;
            }

            _logger.LogDebug("Seat found by row and number: {Row}{Number} (ID: {SeatId})",
                dto.Row, dto.Number, entity.Id);
            return _mapper.Map<SeatResponse>(entity);
        }

        public async Task<IEnumerable<SeatResponse>?> GetSeatsByRoomIdAsync(int roomId)
        {
            _logger.LogDebug("Getting seats by room ID: {RoomId}", roomId);

            var entity = await _unitOfWork.SeatRepository.GetSeatsByRoomIdAsync(roomId);

            if (entity == null)
            {
                _logger.LogWarning("No seats found for room ID: {RoomId}", roomId);
                return null;
            }

            _logger.LogDebug("Retrieved {SeatCount} seats for room ID: {RoomId}", entity.Count(), roomId);
            return _mapper.Map<IEnumerable<SeatResponse>>(entity);
        }

        public async Task<SeatResponse?> OccupySeatAsync(int id)
        {
            _logger.LogInformation("Occupying seat: {SeatId}", id);

            var seat = await _unitOfWork.SeatRepository.GetByIdAsync(id);
            if (seat == null)
            {
                _logger.LogWarning("Seat not found for occupation: {SeatId}", id);
                return null;
            }

            seat.OccupySeat(id);
            await _unitOfWork.SeatRepository.UpdateAsync(seat);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Seat occupied successfully: {SeatId}", id);
            return _mapper.Map<SeatResponse>(seat);
        }

        public async Task<SeatResponse?> FreeSeatAsync(int id)
        {
            _logger.LogInformation("Freeing seat: {SeatId}", id);

            var seat = await _unitOfWork.SeatRepository.GetByIdAsync(id);
            if (seat == null)
            {
                _logger.LogWarning("Seat not found for freeing: {SeatId}", id);
                return null;
            }

            seat.FreeSeat(id);
            await _unitOfWork.SeatRepository.UpdateAsync(seat);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Seat freed successfully: {SeatId}", id);
            return _mapper.Map<SeatResponse>(seat);
        }
    }
}