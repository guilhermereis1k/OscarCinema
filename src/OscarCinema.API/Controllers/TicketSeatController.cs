using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OscarCinema.Application.DTOs.TicketSeat;
using OscarCinema.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace OscarCinema.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketSeatController : ControllerBase
    {
        private readonly ITicketSeatService _ticketSeatService;
        private readonly ILogger<TicketSeatController> _logger;

        public TicketSeatController(ITicketSeatService ticketSeatService, ILogger<TicketSeatController> logger)
        {
            _ticketSeatService = ticketSeatService;
            _logger = logger;
        }

        [Authorize]
        [HttpPut("{ticketId:int}/seat/{seatId:int}/price")]
        public async Task<ActionResult<TicketSeatResponse>> UpdatePrice(int ticketId, int seatId, [FromBody] decimal newPrice)
        {
            _logger.LogInformation("Updating price for TicketId {TicketId}, SeatId {SeatId} to {NewPrice}", ticketId, seatId, newPrice);

            var updatedSeat = await _ticketSeatService.UpdatePriceAsync(ticketId, seatId, newPrice);

            return Ok(updatedSeat);
        }
    }
}
