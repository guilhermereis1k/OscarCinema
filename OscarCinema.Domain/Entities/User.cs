using OscarCinema.Domain.Enums.Movie;
using OscarCinema.Domain.Enums.User;
using OscarCinema.Domain.Validation;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace OscarCinema.Domain.Entities
{
    public class User
    {
        [Required]
        public int Id { get; private set; }

        [Required]
        public string Name { get; private set; }

        [Required]
        public string DocumentNumber { get; private set; }

        [Required]
        [EmailAddress]
        public string Email { get; private set; }

        [Required]
        [MinLength(8)]
        public string Password { get; private set; }
        public UserRole Role { get; private set; }


        private List<Ticket> _tickets = new();
        public IReadOnlyList<Ticket> Tickets => _tickets.AsReadOnly();

        public User() { }

        public User(string name, string documentNumber, string email, string password, UserRole role)
        {
            ValidateDomain(name, documentNumber, email, password, role);

            Name = name;
            DocumentNumber = documentNumber;
            Email = email;
            Password = password;
            Role = role;
        }

        public void Update(string name, string documentNumber, string email, string password, UserRole role)
        {
            ValidateDomain(name, documentNumber, email, password, role);

            Name = name;
            DocumentNumber = documentNumber;
            Email = email;
            Password = password;
            Role = role;
        }

        public void AddTicket(Ticket ticket)
        {
            DomainExceptionValidation.When(ticket == null, "Ticket cannot be null");
            _tickets.Add(ticket);
        }

        private void ValidateDomain(string name, string documentNumber, string email, string password, UserRole role)
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

            DomainExceptionValidation.When(string.IsNullOrWhiteSpace(password),
                "Password is required.");

            DomainExceptionValidation.When(password.Length < 6,
                "Password must be at least 6 characters long.");

            DomainExceptionValidation.When(!Enum.IsDefined(typeof(UserRole), role),
                "Role is required.");
        }
    }
}
