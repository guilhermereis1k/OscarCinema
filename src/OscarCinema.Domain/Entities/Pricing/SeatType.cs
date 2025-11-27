using OscarCinema.Domain.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Domain.Entities.Pricing
{
    public class SeatType
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public decimal Price { get; private set; }

        public SeatType(){ }

        public SeatType(string name, string description, decimal price, bool isActive)
        {
            Name = name;
            Description = description;
            Price = price;
        }

        public SeatType(string name, string description, decimal price)
        {
            Name = name;
            Description = description;
            Price = price;

        }

        public void UpdatePrice(decimal newPrice)
        {
            DomainExceptionValidation.When(newPrice <= 0, "Price must be positive");
            Price = newPrice;
        }

        public void Update(string name, string description)
        {
            ValidateDomain(name, description);

            Name = name;
            Description = description;
        }

        private void ValidateDomain(string name, string description)
        {
            DomainExceptionValidation.When(string.IsNullOrWhiteSpace(name),
                "Exhibition type name is required");

            DomainExceptionValidation.When(name.Length < 2,
                "Exhibition type name must be at least 2 characters long");

            DomainExceptionValidation.When(name.Length > 50,
                "Exhibition type name cannot exceed 50 characters");

            DomainExceptionValidation.When(string.IsNullOrWhiteSpace(description),
                "Exhibition type description is required");

            DomainExceptionValidation.When(description.Length < 10,
                "Exhibition type description must be at least 10 characters long");
        }
    }
}
