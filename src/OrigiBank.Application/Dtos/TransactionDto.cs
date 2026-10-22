using OrigiBank.Domain.Enums;
namespace OrigiBank.Application.Dtos;
public sealed record TransactionDto(
    Guid Id,
    TransactionType Type,
    decimal Amount,
    DateTime TimestampUtc,
    decimal BalanceAfter);