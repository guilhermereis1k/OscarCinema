using OscarCinema.Domain.Common.Utils;
using OscarCinema.Domain.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Domain.Common.ValueObjects;

[ComplexType]
public class Cpf 
{
    public string Number { get; private set; }

    internal Cpf()
    {
    }

    public Cpf(string number)
    {
        var cleaned = CpfUtils.Clean(number);

        if (!CpfUtils.IsValid(cleaned))
            throw new DomainExceptionValidation("CPF inválido");

        Number = cleaned;
    }

    public string Formatted => CpfUtils.Format(Number);

    public static bool IsValid(string cpf) => CpfUtils.IsValid(cpf);

    public override bool Equals(object obj)
    {
        return obj is Cpf cpf && Number == cpf.Number;
    }

    public override int GetHashCode()
    {
        return Number?.GetHashCode() ?? 0;
    }

    public override string ToString() => Formatted;

    public static bool operator ==(Cpf left, Cpf right)
    {
        if (left is null ^ right is null)
            return false;

        return left?.Equals(right!) != false;
    }

    public static bool operator !=(Cpf left, Cpf right)
    {
        return !(left == right);
    }
}