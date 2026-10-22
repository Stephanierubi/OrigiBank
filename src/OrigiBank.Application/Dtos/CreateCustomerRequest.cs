using OrigiBank.Domain.Enums;
namespace OrigiBank.Application.Dtos;
public sealed record CreateCustomerRequest(
    string FullName,
    DateOnly DateOfBirth,
    Gender Gender,
    decimal MonthlyIncome);