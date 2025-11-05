using OscarCinema.Domain.Common.Utils;
using OscarCinema.Domain.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Domain.Common.ValueObjects;

public class Cpf : ValueObject
{
    public string Number { get; private set; }

    public Cpf(string number)
    {
        var cleaned = CpfUtils.Clean(number);

        if (!CpfUtils.IsValid(cleaned))
            throw new DomainExceptionValidation("CPF inválido");

        Number = cleaned;
    }

    public string Formatted => CpfUtils.Format(Number);

    public static bool IsValid(string cpf) => CpfUtils.IsValid(cpf);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Number;
    }

    public override string ToString() => Formatted;
}