using OscarCinema.Domain.Common.ValueObjects;
using OscarCinema.Domain.Enums.Movie;
using OscarCinema.Domain.Enums.User;
using OscarCinema.Domain.Validation;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace OscarCinema.Domain.Entities
{
    public class User
    {
        public int Id { get; internal set; }

        public string Name { get; private set; }
        public Cpf DocumentNumber { get; private set; }
        public string Email { get; private set; }
        public UserRole Role { get; private set; }


        private readonly List<Ticket> _tickets;
        public IReadOnlyList<Ticket> Tickets => _tickets.AsReadOnly();

        public User() {
            _tickets = new List<Ticket>();
        }

        public User(string name, string documentNumber, string email, UserRole role)
        {
            ValidateDomain(name, documentNumber, email, role);

            Name = name;
            DocumentNumber = new Cpf(documentNumber);
            Email = email;
            Role = role;
            _tickets = new List<Ticket>();
        }

        public void Update(string name, string documentNumber, string email, UserRole role)
        {
            ValidateDomain(name, documentNumber, email, role);

            Name = name;
            DocumentNumber = new Cpf(documentNumber);
            Email = email;
            Role = role;
        }

        public void AddTicket(Ticket ticket)
        {
            DomainExceptionValidation.When(ticket == null, "Ticket cannot be null");
            _tickets.Add(ticket);
        }

        private void ValidateDomain(string name, string documentNumber, string email, UserRole role)
        {
            DomainExceptionValidation.When(string.IsNullOrWhiteSpace(name),
                "Name is required.");

            DomainExceptionValidation.When(name.Length < 2,
                "Name must be at least 2 characters long.");

            DomainExceptionValidation.When(string.IsNullOrWhiteSpace(documentNumber),
                "Document number is required.");

            DomainExceptionValidation.When(string.IsNullOrWhiteSpace(email),
                "Email is required.");

            DomainExceptionValidation.When(!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"),
                "Email format is invalid.");

            DomainExceptionValidation.When(!Enum.IsDefined(typeof(UserRole), role),
                "Role is required.");
        }
    }
}
