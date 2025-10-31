using OscarCinema.Domain.Entities.Pricing;
using OscarCinema.Domain.Enums.Ticket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.Interfaces
{
    public interface IPricingService
    {
        decimal CalculateSeatPrice(ExhibitionType exhibitionType, SeatType seatType);
        decimal CalculateTotalPrice(IEnumerable<decimal> seatPrices);
    }
}
