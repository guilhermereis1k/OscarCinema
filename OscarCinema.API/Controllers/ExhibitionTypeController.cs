using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OscarCinema.Application.DTOs.ExhibitionType;
using OscarCinema.Application.Interfaces;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Infrastructure.Repositories;

namespace OscarCinema.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExhibitionTypeController : ControllerBase
    {
        private readonly IExhibitionTypeService _exhibitionTypeService;

        public ExhibitionTypeController(IExhibitionTypeService exhibitionTypeService)
        {
            _exhibitionTypeService = exhibitionTypeService;

        }

        [HttpPost]
        public async Task<ActionResult<ExhibitionTypeResponseDTO>> Create([FromBody] CreateExhibitionTypeDTO dto)
        {
            try
            {
                var createdExhibitionType = await _exhibitionTypeService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = createdExhibitionType.Id },
                    createdExhibitionType);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ExhibitionTypeResponseDTO>> GetById(int id)
        {
            var ExhibitionType = await _exhibitionTypeService.GetByIdAsync(id);

            if (ExhibitionType == null)
                return NotFound();

            return Ok(ExhibitionType);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExhibitionTypeResponseDTO>>> GetAll()
        {
            var ExhibitionTypeList = await _exhibitionTypeService.GetAllAsync();

            return Ok(ExhibitionTypeList);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ExhibitionTypeResponseDTO>> Update(int id, [FromBody] UpdateExhibitionTypeDTO dto)
        {
            try
            {
                var updatedExhibitionType = await _exhibitionTypeService.UpdateAsync(id, dto);
                return Ok(updatedExhibitionType);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _exhibitionTypeService.DeleteAsync(id);

            if (!deleted)
                return NotFound($"ExhibitionType with ID {id} not found");

            return NoContent();
        }
    }
}

