using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OscarCinema.Application.DTOs;
using OscarCinema.Application.DTOs.SeatType;
using OscarCinema.Application.Interfaces;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Infrastructure.Repositories;

namespace OscarCinema.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeatTypeController : ControllerBase
    {
        private readonly ISeatTypeService _seatTypeService;

        public SeatTypeController(ISeatTypeService SeatTypeService)
        {
            _seatTypeService = SeatTypeService;

        }

        [HttpPost]
        public async Task<ActionResult<SeatTypeResponseDTO>> Create([FromBody] CreateSeatTypeDTO dto)
        {
            try
            {
                var createdSeatType = await _seatTypeService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = createdSeatType.Id },
                    createdSeatType);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SeatTypeResponseDTO>> GetById(int id)
        {
            var seatType = await _seatTypeService.GetByIdAsync(id);

            if (seatType == null)
                return NotFound();

            return Ok(seatType);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SeatTypeResponseDTO>>> GetAll()
        {
            var seatTypeList = await _seatTypeService.GetAllAsync();

            return Ok(seatTypeList);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<SeatTypeResponseDTO>> Update(int id, [FromBody] UpdateSeatTypeDTO dto)
        {
            try
            {
                var updatedSeatType = await _seatTypeService.UpdateAsync(id, dto);
                return Ok(updatedSeatType);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _seatTypeService.DeleteAsync(id);

            if (!deleted)
                return NotFound($"SeatType with ID {id} not found");

            return NoContent();
        }
    }
}

