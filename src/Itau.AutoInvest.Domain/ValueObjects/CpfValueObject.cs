namespace Itau.AutoInvest.Domain.ValueObjects;

using System;
using System.Linq;
using System.Text.RegularExpressions;

public sealed class CpfValueObject
{
    public string Number { get; }

    public CpfValueObject(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            throw new ArgumentException("O CPF não pode estar vazio.");

        var cleanCpf = new string(cpf.Where(char.IsDigit).ToArray());

        if (!Validate(cleanCpf))
            throw new ArgumentException("O CPF fornecido é inválido.");

        Number = cleanCpf;
    }

    private static bool Validate(string cpf)
    {
        if (cpf.Length != 11) return false;

        if (cpf.Distinct().Count() == 1) return false;

        int[] multiplier1 = [10, 9, 8, 7, 6, 5, 4, 3, 2];
        int[] multiplier2 = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2];

        string tempCpf = cpf.Substring(0, 9);
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
        digit += remainder;

        return cpf.EndsWith(digit);
    }

    public string GetFormatted() => 
        Regex.Replace(Number, @"(\d{3})(\d{3})(\d{3})(\d{2})", "$1.$2.$3-$4");

    public override string ToString() => Number;

    public override bool Equals(object obj) => 
        obj is CpfValueObject other && Number == other.Number;

    public override int GetHashCode() => Number.GetHashCode();
}