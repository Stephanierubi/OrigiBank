namespace OrigiBank.Domain.Exceptions;
public sealed class InsufficientFundsException(decimal balance, decimal amount)
    : BusinessException($"Fondos insuficientes. Saldo actual: {balance:N2}, monto solicitado: {amount:N2}.")
{
    public decimal Balance { get; } = balance;
    public decimal Amount { get; } = amount;
}