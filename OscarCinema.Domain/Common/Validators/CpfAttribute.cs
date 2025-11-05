using OscarCinema.Domain.Common.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Domain.Common.Validators
{
    public class CpfAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return ValidationResult.Success; // RequiredAttribute cuida disso

            var cpf = value.ToString();

            if (!CpfUtils.IsValid(cpf))
                return new ValidationResult("CPF inválido");

            return ValidationResult.Success;
        }
    }
}
