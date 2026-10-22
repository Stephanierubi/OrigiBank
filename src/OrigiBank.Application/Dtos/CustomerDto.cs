using OrigiBank.Domain.Enums;
namespace OrigiBank.Application.Dtos;
public sealed record CustomerDto(
    Guid Id,
    string FullName,
    DateOnly DateOfBirth,
    Gender Gender,
    decimal MonthlyIncome);