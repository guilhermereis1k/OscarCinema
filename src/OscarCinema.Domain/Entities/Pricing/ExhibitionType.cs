using OscarCinema.Domain.Validation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Domain.Entities.Pricing
{
    public class ExhibitionType
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string TechnicalSpecs { get; private set; }

        public decimal Price { get; private set; }
        public bool IsActive { get; private set; } = true;

        public ExhibitionType(){ }

        public ExhibitionType(string name, string description, string technicalSpecs, decimal price)
        {
            ValidateDomain(name, description, technicalSpecs);

            Name = name;
            Description = description;
            TechnicalSpecs = technicalSpecs;
            Price = price;
            IsActive = true;
        }

        public void UpdatePrice(decimal newPrice)
        {
            DomainExceptionValidation.When(newPrice <= 0, "Price must be positive");
            Price = newPrice;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Reactivate()
        {
            IsActive = true;
        }

        public void Update(string name, string description, string technicalSpecs, bool isActive)
        {
            ValidateDomain(name, description, technicalSpecs);

            Name = name;
            Description = description;
            IsActive = isActive;
            TechnicalSpecs = technicalSpecs;
        }

        private void ValidateDomain(string name, string description, string technicalSpecs)
        {
            DomainExceptionValidation.When(string.IsNullOrWhiteSpace(name),
                "Exhibition type name is required");

            DomainExceptionValidation.When(name.Length < 2,
                "Exhibition type name must be at least 2 characters long");

            DomainExceptionValidation.When(name.Length > 50,
                "Exhibition type name cannot exceed 50 characters");

            DomainExceptionValidation.When(string.IsNullOrWhiteSpace(description),
                "Exhibition type description is required");

            DomainExceptionValidation.When(description.Length < 5,
                "Exhibition type description must be at least 5 characters long");

            DomainExceptionValidation.When(string.IsNullOrWhiteSpace(technicalSpecs),
                "Technical specifications are required");
        }
    }
}
