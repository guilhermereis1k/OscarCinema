using OscarCinema.Domain.Enums.Ticket;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OscarCinema.Application.Interfaces
{
    public interface IPricingService
    {
        Task<decimal> CalculateSeatPriceAsync(int exhibitionTypeId, int seatTypeId);

        decimal ApplyTicketType(decimal basePrice, TicketType type);

        decimal CalculateTotalPrice(IEnumerable<decimal> seatPrices);
    }
}
