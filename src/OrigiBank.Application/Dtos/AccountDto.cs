namespace OrigiBank.Application.Dtos;
public sealed record AccountDto(
    string AccountNumber,
    Guid CustomerId,
    decimal Balance);