using OscarCinema.Domain.Common.Validators;
using OscarCinema.Domain.Enums.Movie;
using OscarCinema.Domain.Enums.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.DTOs.User
{
    public class RegisterUserDTO
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome não pode ter mais de 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "CPF é obrigatório")]
        [Cpf]
        public string DocumentNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é obrigatória")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Senha deve ter entre 6 e 100 caracteres")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tipo de usuário é obrigatório")]
        public UserRole Role { get; set; }
    }
}
