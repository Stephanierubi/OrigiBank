using OrigiBank.Domain.Enums;
using OrigiBank.Domain.Exceptions;

namespace OrigiBank.Domain.Entities;

public class Customer
{
    private const int MaxFullNameLength = 150;

    public Guid Id { get; private set; } = Guid.NewGuid();

    public string FullName { get; private set; } = string.Empty;

    public DateOnly DateOfBirth { get; private set; }

    public Gender Gender { get; private set; }

    public decimal MonthlyIncome { get; private set; }

    // Constructor privado requerido para la materialización de EF Core.
    private Customer()
    {
    }

    public Customer(
        string fullName,
        DateOnly dateOfBirth,
        Gender gender,
        decimal monthlyIncome,
        DateOnly today)
    {
        FullName = ValidateFullName(fullName);
        DateOfBirth = ValidateDateOfBirth(dateOfBirth, today);
        Gender = ValidateGender(gender);
        MonthlyIncome = ValidateMonthlyIncome(monthlyIncome);
    }

    private static string ValidateFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ValidationException(
                "El nombre completo es obligatorio.");
        }

        var normalizedName = fullName.Trim();

        if (normalizedName.Length > MaxFullNameLength)
        {
            throw new ValidationException(
                $"El nombre completo no puede superar " +
                $"{MaxFullNameLength} caracteres.");
        }

        return normalizedName;
    }

    private static DateOnly ValidateDateOfBirth(
        DateOnly dateOfBirth,
        DateOnly today)
    {
        if (dateOfBirth == DateOnly.MinValue)
        {
            throw new ValidationException("La fecha de nacimiento es obligatoria.");
        }

        if (dateOfBirth >= today)
        {
            throw new ValidationException("La fecha de nacimiento debe ser anterior a hoy.");
        }

        return dateOfBirth;
    }

    private static Gender ValidateGender(Gender gender)
    {
        if (!Enum.IsDefined(gender))
        {
            throw new ValidationException("El sexo/género indicado no es válido.");
        }

        return gender;
    }

    private static decimal ValidateMonthlyIncome(decimal monthlyIncome)
    {
        if (monthlyIncome < 0)
        {
            throw new ValidationException("Los ingresos mensuales no pueden ser negativos.");
        }

        return monthlyIncome;
    }
}