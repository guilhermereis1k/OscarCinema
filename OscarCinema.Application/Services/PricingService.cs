using OscarCinema.Application.Interfaces;
using OscarCinema.Domain.Entities.Pricing;
using OscarCinema.Domain.Enums.Ticket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.Services
{
    public class PricingService : IPricingService
    {
        public decimal CalculateSeatPrice(ExhibitionType exhibitionType, SeatType seatType)
        {
            var basePrice = seatType.Price + exhibitionType.Price;
            return basePrice;
        }

        public decimal CalculateTotalPrice(IEnumerable<decimal> seatPrices)
        {
            return seatPrices.Sum();
        }
    }
}
