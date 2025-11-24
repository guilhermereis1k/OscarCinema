using OscarCinema.Domain.Common.Validators;
using OscarCinema.Domain.Enums.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.DTOs.User
{
    public class CreateUser
    {
        [Required]
        public int ApplicationUserId { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome não pode ter mais de 100 caracteres")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "CPF é obrigatório")]
        [Cpf(ErrorMessage = "CPF inválido")]
        public string DocumentNumber { get; set; } = null!;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = null!;


        [Required(ErrorMessage = "Tipo de usuário é obrigatório")]
        public UserRole Role { get; set; }
    }
}
