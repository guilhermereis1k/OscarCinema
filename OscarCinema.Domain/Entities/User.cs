using OscarCinema.Domain.Enums;
using OscarCinema.Domain.Validation;
using System.Text.RegularExpressions;

namespace OscarCinema.Domain.Entities
{
    public class User
    {
        public int UserId { get; private set; }
        public string Name { get; private set; }
        public string DocumentNumber { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public UserRole Role { get; private set; }

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

            DomainExceptionValidation.When(string.IsNullOrWhiteSpace(role),
                "Role is required.");
        }
    }
}
