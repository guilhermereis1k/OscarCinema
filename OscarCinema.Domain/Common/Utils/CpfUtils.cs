using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Domain.Common.Utils;

public static class CpfUtils
{
    public static bool IsValid(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        var cleaned = Clean(cpf);

        if (cleaned.Length != 11)
            return false;

        // Verifica sequência repetida
        if (IsRepeatedSequence(cleaned))
            return false;

        int[] multiplier1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplier2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        string tempCpf = cleaned.Substring(0, 9);
        int sum = 0;

        for (int i = 0; i < 9; i++)
            sum += int.Parse(tempCpf[i].ToString()) * multiplier1[i];

        int remainder = sum % 11;
        remainder = remainder < 2 ? 0 : 11 - remainder;

        string digit = remainder.ToString();
        tempCpf += digit;
        sum = 0;

        for (int i = 0; i < 10; i++)
            sum += int.Parse(tempCpf[i].ToString()) * multiplier2[i];

        remainder = sum % 11;
        remainder = remainder < 2 ? 0 : 11 - remainder;

        digit += remainder.ToString();

        return cleaned.EndsWith(digit);
    }

    public static string Format(string cpf)
    {
        var cleaned = Clean(cpf);
        return cleaned.Length == 11 ?
            Convert.ToUInt64(cleaned).ToString(@"000\.000\.000\-00") :
            cleaned;
    }

    public static string Clean(string cpf)
    {
        return string.IsNullOrWhiteSpace(cpf) ?
            string.Empty :
            new string(cpf.Where(char.IsDigit).ToArray());
    }

    private static bool IsRepeatedSequence(string input)
    {
        return input.All(c => c == input[0]);
    }
}