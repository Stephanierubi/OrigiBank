namespace OrigiBank.Application.Dtos;
public sealed record CreateAccountRequest(
    Guid CustomerId,
    decimal InitialBalance);