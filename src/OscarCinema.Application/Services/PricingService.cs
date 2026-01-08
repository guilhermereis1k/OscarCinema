using Microsoft.Extensions.Logging;
using OscarCinema.Application.Interfaces;
using OscarCinema.Domain.Enums.Ticket;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Domain.Validation;

namespace OscarCinema.Application.Services
{
    public class PricingService : IPricingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PricingService> _logger;

        public PricingService(
            IUnitOfWork unitOfWork,
            ILogger<PricingService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<decimal> CalculateSeatPriceAsync(
            int exhibitionTypeId,
            int seatTypeId)
        {
            var exhibitionType = await _unitOfWork.ExhibitionTypeRepository
                .GetByIdAsync(exhibitionTypeId);

            DomainExceptionValidation.When(
                exhibitionType == null,
                $"ExhibitionType {exhibitionTypeId} not found"
            );

            var seatType = await _unitOfWork.SeatTypeRepository
                .GetByIdAsync(seatTypeId);

            DomainExceptionValidation.When(
                seatType == null,
                $"SeatType {seatTypeId} not found"
            );

            _logger.LogDebug(
                "Calculating seat price | ExhibitionType: {ExhibitionType} ({ExhibitionPrice}) | SeatType: {SeatType} ({SeatPrice})",
                exhibitionType.Name,
                exhibitionType.Price,
                seatType.Name,
                seatType.Price
            );

            var basePrice = exhibitionType.Price + seatType.Price;

            _logger.LogDebug(
                "Calculated base seat price: {Price}",
                basePrice
            );

            return basePrice;
        }


        public decimal ApplyTicketType(decimal basePrice, TicketType type)
        {
            DomainExceptionValidation.When(
                basePrice <= 0,
                "Base price must be positive"
            );

            var multiplier = type switch
            {
                TicketType.Full => 1.0m,
                TicketType.Half => 0.5m,
                TicketType.StudentHalf => 0.5m,
                _ => throw new DomainExceptionValidation("Invalid TicketType")
            };

            var finalPrice = basePrice * multiplier;

            _logger.LogDebug(
                "Applied ticket type {TicketType} | Final price: {FinalPrice}",
                type,
                finalPrice
            );

            return finalPrice;
        }

        public decimal CalculateTotalPrice(IEnumerable<decimal> seatPrices)
        {
            DomainExceptionValidation.When(
                seatPrices == null || !seatPrices.Any(),
                "Seat prices collection is empty"
            );

            var totalPrice = seatPrices.Sum();

            _logger.LogDebug(
                "Calculated total ticket price: {TotalPrice}",
                totalPrice
            );

            return totalPrice;
        }
    }
}
