namespace OrigiBank.Application.Dtos;
public sealed record BalanceDto(
    string AccountNumber,
    decimal Balance);