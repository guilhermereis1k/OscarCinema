using Microsoft.Extensions.Logging;
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
        private readonly ILogger<PricingService> _logger;

        public PricingService(ILogger<PricingService> logger)
        {
            _logger = logger;
        }

        public decimal CalculateSeatPrice(ExhibitionType exhibitionType, SeatType seatType)
        {
            _logger.LogDebug("Calculating seat price for exhibition type: {ExhibitionType} (Price: {ExhibitionPrice}) and seat type: {SeatType} (Price: {SeatPrice})",
                exhibitionType.Name, exhibitionType.Price, seatType.Name, seatType.Price);

            var basePrice = seatType.Price + exhibitionType.Price;

            _logger.LogDebug("Calculated seat price: {Price}", basePrice);
            return basePrice;
        }

        public decimal ApplyTicketType(decimal basePrice, TicketType type)
        {
            decimal multiplier = type switch
            {
                TicketType.Full => 1.0m,
                TicketType.Half => 0.5m,
                TicketType.StudentHalf => 0.5m,
            };

            return basePrice * multiplier;
        }

        public decimal CalculateTotalPrice(IEnumerable<decimal> seatPrices)
        {
            _logger.LogDebug("Calculating total price for {SeatCount} seats", seatPrices.Count());

            var totalPrice = seatPrices.Sum();

            _logger.LogDebug("Calculated total price: {TotalPrice}", totalPrice);
            return totalPrice;
        }
    }
}