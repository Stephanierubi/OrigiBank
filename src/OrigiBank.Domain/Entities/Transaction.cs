using OrigiBank.Domain.Enums;
using OrigiBank.Domain.Exceptions;

namespace OrigiBank.Domain.Entities;

public class Transaction
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();

    public Guid AccountId { get; private set; }

    public TransactionType Type { get; private set; }

    public decimal Amount { get; private set; }

    public decimal BalanceAfter { get; private set; }

    public DateTime TimestampUtc { get; private set; }

    // Requerido por Entity Framework Core.
    private Transaction()
    {
    }

    // Solo la entidad Account puede crear transacciones directamente.
    internal Transaction(
        Guid accountId,
        TransactionType type,
        decimal amount,
        decimal balanceAfter,
        DateTime timestampUtc)
    {
        ValidateAccountId(accountId);
        ValidateType(type);
        ValidateAmount(amount);
        ValidateBalanceAfter(balanceAfter);
        ValidateTimestamp(timestampUtc);

        AccountId = accountId;
        Type = type;
        Amount = amount;
        BalanceAfter = balanceAfter;
        TimestampUtc = timestampUtc;
    }

    private static void ValidateAccountId(Guid accountId)
    {
        if (accountId == Guid.Empty)
        {
            throw new ValidationException(
                "El identificador de la cuenta es obligatorio.");
        }
    }

    private static void ValidateType(TransactionType type)
    {
        if (!Enum.IsDefined(type))
        {
            throw new ValidationException(
                "El tipo de transacción no es válido.");
        }
    }

    private static void ValidateAmount(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ValidationException(
                "El monto de la transacción debe ser mayor que cero.");
        }
    }

    private static void ValidateBalanceAfter(decimal balanceAfter)
    {
        if (balanceAfter < 0)
        {
            throw new ValidationException(
                "El saldo resultante no puede ser negativo.");
        }
    }

    private static void ValidateTimestamp(DateTime timestampUtc)
    {
        if (timestampUtc.Kind != DateTimeKind.Utc)
        {
            throw new ValidationException(
                "La fecha de la transacción debe estar expresada en UTC.");
        }
    }
}

