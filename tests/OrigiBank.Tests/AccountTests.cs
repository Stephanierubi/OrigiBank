using OrigiBank.Domain.Entities;
using OrigiBank.Domain.Enums;
using OrigiBank.Domain.Exceptions;

namespace OrigiBank.Tests;

public class AccountTests
{
    private static readonly DateTime Now = new(2026, 6, 18, 10, 0, 0, DateTimeKind.Utc);

    private static Account CreateAccount(decimal balance = 100m) =>
        new(Guid.NewGuid(), "ACC-20260618-0001", balance);

    [Fact]
    public void Deposit_IncreasesBalance_AndRecordsHistoricalBalance()
    {
        var account = CreateAccount(100m);

        var tx = account.Deposit(50m, Now);

        Assert.Equal(150m, account.Balance);
        Assert.Equal(TransactionType.Deposit, tx.Type);
        Assert.Equal(50m, tx.Amount);
        Assert.Equal(150m, tx.BalanceAfter);
    }

    [Fact]
    public void Withdraw_WithSufficientFunds_DecreasesBalance()
    {
        var account = CreateAccount(100m);

        var tx = account.Withdraw(30m, Now);

        Assert.Equal(70m, account.Balance);
        Assert.Equal(TransactionType.Withdrawal, tx.Type);
        Assert.Equal(70m, tx.BalanceAfter);
    }

    [Fact]
    public void Withdraw_WithInsufficientFunds_ThrowsAndKeepsBalance()
    {
        var account = CreateAccount(100m);

        var ex = Assert.Throws<InsufficientFundsException>(() => account.Withdraw(150m, Now));

        Assert.Equal(100m, ex.Balance);
        Assert.Equal(150m, ex.Amount);
        Assert.Equal(100m, account.Balance);
        Assert.Empty(account.Transactions);
    }

    [Fact]
    public void Withdraw_ExactBalance_LeavesZero()
    {
        var account = CreateAccount(100m);

        account.Withdraw(100m, Now);

        Assert.Equal(0m, account.Balance);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Deposit_WithNonPositiveAmount_Throws(decimal amount)
    {
        var account = CreateAccount();

        Assert.Throws<ValidationException>(() => account.Deposit(amount, Now));
    }

    [Fact]
    public void Transactions_AreRecordedInOrder_WithHistoricalBalances()
    {
        var account = CreateAccount(100m);

        account.Deposit(50m, Now);
        account.Withdraw(20m, Now.AddMinutes(1));

        var balances = account.Transactions.Select(t => t.BalanceAfter).ToList();
        Assert.Equal([150m, 130m], balances);
    }
}