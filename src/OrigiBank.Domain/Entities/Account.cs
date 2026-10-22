using OrigiBank.Domain.Enums;
using OrigiBank.Domain.Exceptions;
namespace OrigiBank.Domain.Entities;
public class Account
{
    private readonly List<Transaction> _transactions = [];
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string AccountNumber { get; private set; } = default!;
    public Guid CustomerId { get; private set; }
    public decimal Balance { get; private set; }
    // Token de concurrencia optimista 
    public int Version { get; private set; }
    // Encapsulamiento: colección de solo lectura 
    public IReadOnlyCollection<Transaction> Transactions => _transactions.AsReadOnly();
    // Requerido por Entity Framework Core para la materialización
    private Account()
    {
    }
    public Account(Guid customerId, string accountNumber, decimal initialBalance)
    {
        if (customerId == Guid.Empty)
        {
            throw new ValidationException("El identificador del cliente es obligatorio.");
        }
        if (string.IsNullOrWhiteSpace(accountNumber))
        {
            throw new ValidationException("El número de cuenta es obligatorio.");
        }
        if (initialBalance < 0)
        {
            throw new ValidationException("El saldo inicial no puede ser negativo.");
        }
        CustomerId = customerId;
        AccountNumber = accountNumber.Trim();
        Balance = initialBalance;
    }
    public Transaction Deposit(decimal amount, DateTime utcNow)
    {
        EnsurePositiveAmount(amount);
        var newBalance = Balance + amount;
        return ApplyTransaction(TransactionType.Deposit, amount, newBalance, utcNow);
    }
    public Transaction Withdraw(decimal amount, DateTime utcNow)
    {
        EnsurePositiveAmount(amount);
        if (Balance < amount)
        {
            // Rechazo mediante excepción 
            throw new InsufficientFundsException(Balance, amount);
        }
        var newBalance = Balance - amount;
        return ApplyTransaction(TransactionType.Withdrawal, amount, newBalance, utcNow);
    }
    private Transaction ApplyTransaction(
        TransactionType type,
        decimal amount,
        decimal newBalance,
        DateTime utcNow)
    {
        Balance = newBalance;
        Version++;
        // Instancia la transacción pasando el saldo histórico resultante (BalanceAfter)
        var transaction = new Transaction(Id, type, amount, Balance, utcNow);
        _transactions.Add(transaction);
        return transaction;
    }
    private static void EnsurePositiveAmount(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ValidationException("El monto de la transacción debe ser mayor que cero.");
        }
    }
}
