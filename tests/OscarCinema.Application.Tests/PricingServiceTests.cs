using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable;
using MockQueryable.Moq;
using Moq;
using OscarCinema.Application.DTOs.Pagination;
using OscarCinema.Application.DTOs.Ticket;
using OscarCinema.Application.DTOs.TicketSeat;
using OscarCinema.Application.DTOs.User;
using OscarCinema.Application.Interfaces;
using OscarCinema.Application.Services;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Entities.Pricing;
using OscarCinema.Domain.Enums.Ticket;
using OscarCinema.Domain.Enums.User;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Domain.Validation;
using Xunit;

namespace OscarCinema.Application.Tests
{
    public class PricingServiceTests
    {
        private readonly Mock<ILogger<PricingService>> _loggerMock;
        private readonly PricingService _service;

        public PricingServiceTests()
        {
            _loggerMock = new Mock<ILogger<PricingService>>();
            _service = new PricingService(_loggerMock.Object);
        }

        [Fact]
        public void CalculateSeatPrice_ShouldReturnCorrectPrice()
        {
            var exhibitionType = new ExhibitionType("IMAX", "Big screen", "4K HDR", 50m);
            var seatType = new SeatType(
                "VIP",
                "Large seat with premium comfort",
                50.50m
            );

            var result = _service.CalculateSeatPrice(exhibitionType, seatType);

            result.Should().Be(100.50m);
        }

        [Fact]
        public void CalculateSeatPrice_ShouldHandleZeroPrices()
        {
            var exhibitionType = new ExhibitionType("IMAX", "Big screen", "4K HDR", 0m);
            var seatType = new SeatType(
                "VIP",
                "Large seat with premium comfort",
                0m
            );

            var result = _service.CalculateSeatPrice(exhibitionType, seatType);

            result.Should().Be(0.00m);
        }

        [Fact]
        public void CalculateTotalPrice_ShouldReturnSumOfSeatPrices()
        {
            var seatPrices = new List<decimal> { 25.00m, 30.00m, 35.00m };

            var result = _service.CalculateTotalPrice(seatPrices);

            result.Should().Be(90.00m);
        }

        [Fact]
        public void CalculateTotalPrice_ShouldHandleEmptyList()
        {
            var seatPrices = new List<decimal>();

            var result = _service.CalculateTotalPrice(seatPrices);

            result.Should().Be(0.00m);
        }

        [Fact]
        public void CalculateTotalPrice_ShouldHandleSingleSeat()
        {
            var seatPrices = new List<decimal> { 25.50m };

            var result = _service.CalculateTotalPrice(seatPrices);

            result.Should().Be(25.50m);
        }

        [Fact]
        public void CalculateTotalPrice_ShouldHandleDecimalPrices()
        {
            var seatPrices = new List<decimal> { 15.75m, 20.25m, 10.50m };

            var result = _service.CalculateTotalPrice(seatPrices);

            result.Should().Be(46.50m);
        }
    }
}

