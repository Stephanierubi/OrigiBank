using OrigiBank.Domain.Entities;
namespace OrigiBank.Application.Dtos;
public static class Mappings
{
    public static CustomerDto ToDto(this Customer customer)
    {
        ArgumentNullException.ThrowIfNull(customer);
        return new CustomerDto(customer.Id, customer.FullName, customer.DateOfBirth, customer.Gender, customer.MonthlyIncome);
    }
    public static AccountDto ToDto(this Account account)
    {
        ArgumentNullException.ThrowIfNull(account);
        return new AccountDto(account.AccountNumber, account.CustomerId, account.Balance);
    }
    public static BalanceDto ToBalanceDto(this Account account)
    {
        ArgumentNullException.ThrowIfNull(account);
        return new BalanceDto(account.AccountNumber, account.Balance);
    }
    public static TransactionDto ToDto(this Transaction transaction)
    {
        ArgumentNullException.ThrowIfNull(transaction);
        return new TransactionDto(transaction.Id, transaction.Type, transaction.Amount, transaction.TimestampUtc, transaction.BalanceAfter);
    }
    public static IReadOnlyList<TransactionDto> ToDto(this IEnumerable<Transaction> transactions)
    {
        ArgumentNullException.ThrowIfNull(transactions);
        return transactions.Select(t => t.ToDto()).ToList();
    }
}