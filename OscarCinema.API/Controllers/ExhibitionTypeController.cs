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
        private readonly ILogger<ExhibitionTypeController> _logger;

        public ExhibitionTypeController(IExhibitionTypeService exhibitionTypeService, ILogger<ExhibitionTypeController> logger)
        {
            _exhibitionTypeService = exhibitionTypeService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<ExhibitionTypeResponseDTO>> Create([FromBody] CreateExhibitionTypeDTO dto)
        {
            _logger.LogInformation("Criando novo tipo de exibição: {Name}", dto.Name);

            var createdExhibitionType = await _exhibitionTypeService.CreateAsync(dto);

            _logger.LogInformation("Tipo de exibição criado com ID: {Id}", createdExhibitionType.Id);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdExhibitionType.Id },
                createdExhibitionType);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ExhibitionTypeResponseDTO>> GetById(int id)
        {
            _logger.LogDebug("Buscando tipo de exibição por ID: {Id}", id);

            var exhibitionType = await _exhibitionTypeService.GetByIdAsync(id);


            if (exhibitionType == null) { 
                _logger.LogWarning("Tipo de exibição não encontrado: {Id}", id);
                return NotFound();
            }

            return Ok(exhibitionType);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExhibitionTypeResponseDTO>>> GetAll()
        {
            _logger.LogDebug("Listando todos os tipos de exibição");

            var exhibitionTypeList = await _exhibitionTypeService.GetAllAsync();

            _logger.LogDebug("Retornando {Count} tipos de exibição", exhibitionTypeList.Count());

            return Ok(exhibitionTypeList);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ExhibitionTypeResponseDTO>> Update(int id, [FromBody] UpdateExhibitionTypeDTO dto)
        {
            _logger.LogInformation("Atualizando tipo de exibição ID: {Id} com dados: {@Dto}", id, dto);

            var updatedExhibitionType = await _exhibitionTypeService.UpdateAsync(id, dto);

            _logger.LogInformation("Tipo de exibição atualizado com sucesso: {Id}", id);

            return Ok(updatedExhibitionType);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            _logger.LogInformation("Excluindo tipo de exibição: {Id}", id);

            var deleted = await _exhibitionTypeService.DeleteAsync(id);

            if (!deleted)
            {
                _logger.LogWarning("Tentativa de excluir tipo de exibição não encontrado: {Id}", id);
                return NotFound($"ExhibitionType with ID {id} not found");
            }

            _logger.LogInformation("Tipo de exibição excluído com sucesso: {Id}", id);
            return NoContent();
        }
    }
}

