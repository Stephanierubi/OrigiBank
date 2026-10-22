using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using OrigiBank.Application.Abstractions;
using OrigiBank.Application.Dtos;
using OrigiBank.Application.Services;
using OrigiBank.Domain.Entities;
using OrigiBank.Domain.Exceptions;

namespace OrigiBank.Tests;

public class AccountServiceTests
{
    private const string Number = "ACC-20260618-0001";

    private readonly IAccountRepository _accounts = Substitute.For<IAccountRepository>();
    private readonly ICustomerRepository _customers = Substitute.For<ICustomerRepository>();
    private readonly IAccountNumberGenerator _generator = Substitute.For<IAccountNumberGenerator>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly FakeTimeProvider _clock = new(new DateTimeOffset(2026, 6, 18, 10, 0, 0, TimeSpan.Zero));

    private AccountService CreateService() => new(_accounts, _customers, _generator, _unitOfWork, _clock);

    private Account GivenAccount(decimal balance)
    {
        var account = new Account(Guid.NewGuid(), Number, balance);
        _accounts.GetByNumberAsync(Number, Arg.Any<CancellationToken>()).Returns(account);
        return account;
    }

    [Fact]
    public async Task Deposit_UpdatesBalance_AndSaves()
    {
        GivenAccount(100m);

        var result = await CreateService().DepositAsync(Number, 50m);

        Assert.Equal(150m, result.BalanceAfter);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Withdraw_WithFunds_UpdatesBalance_AndSaves()
    {
        GivenAccount(100m);

        var result = await CreateService().WithdrawAsync(Number, 40m);

        Assert.Equal(60m, result.BalanceAfter);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Withdraw_WithoutFunds_Throws_AndDoesNotSave()
    {
        var account = GivenAccount(100m);

        await Assert.ThrowsAsync<InsufficientFundsException>(() => CreateService().WithdrawAsync(Number, 500m));

        Assert.Equal(100m, account.Balance);
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetBalance_UnknownAccount_ThrowsNotFound()
    {
        _accounts.GetByNumberAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((Account?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => CreateService().GetBalanceAsync("ACC-00000000-0000"));
    }

    [Fact]
    public async Task Create_UnknownCustomer_ThrowsNotFound()
    {
        _customers.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Customer?)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            CreateService().CreateAsync(new CreateAccountRequest(Guid.NewGuid(), 0m)));
    }

    [Fact]
    public async Task Create_RetriesWhenNumberAlreadyExists()
    {
        var customer = new Customer("Ana Perez", new DateOnly(1990, 5, 10), Domain.Enums.Gender.Female, 1500m, new DateOnly(2026, 6, 18));
        _customers.GetByIdAsync(customer.Id, Arg.Any<CancellationToken>()).Returns(customer);
        _generator.Generate().Returns("ACC-20260618-0001", "ACC-20260618-0002");
        _accounts.ExistsByNumberAsync("ACC-20260618-0001", Arg.Any<CancellationToken>()).Returns(true);
        _accounts.ExistsByNumberAsync("ACC-20260618-0002", Arg.Any<CancellationToken>()).Returns(false);

        var result = await CreateService().CreateAsync(new CreateAccountRequest(customer.Id, 100m));

        Assert.Equal("ACC-20260618-0002", result.AccountNumber);
    }
}