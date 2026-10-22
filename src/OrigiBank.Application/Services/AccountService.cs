using OrigiBank.Application.Abstractions;
using OrigiBank.Application.Dtos;
using OrigiBank.Domain.Entities;
using OrigiBank.Domain.Exceptions;

namespace OrigiBank.Application.Services;

public sealed class AccountService(
    IAccountRepository accounts,
    ICustomerRepository customers,
    IAccountNumberGenerator numberGenerator,
    IUnitOfWork unitOfWork,
    TimeProvider clock) : IAccountService
{
    private const int MaxNumberAttempts = 10;

    public async Task<AccountDto> CreateAsync(CreateAccountRequest request, CancellationToken cancellationToken = default)
    {
        var customer = await customers.GetByIdAsync(request.CustomerId, cancellationToken)
            ?? throw new NotFoundException("Cliente", request.CustomerId.ToString());

        var accountNumber = await GenerateUniqueNumberAsync(cancellationToken);
        var account = new Account(customer.Id, accountNumber, request.InitialBalance);

        await accounts.AddAsync(account, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return account.ToDto();
    }

    public async Task<BalanceDto> GetBalanceAsync(string accountNumber, CancellationToken cancellationToken = default)
    {
        var account = await GetAccountOrThrowAsync(accountNumber, cancellationToken);
        return account.ToBalanceDto();
    }

    public async Task<TransactionDto> DepositAsync(string accountNumber, decimal amount, CancellationToken cancellationToken = default)
    {
        var account = await GetAccountOrThrowAsync(accountNumber, cancellationToken);

        var transaction = account.Deposit(amount, clock.GetUtcNow().UtcDateTime);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return transaction.ToDto();
    }

    public async Task<TransactionDto> WithdrawAsync(string accountNumber, decimal amount, CancellationToken cancellationToken = default)
    {
        var account = await GetAccountOrThrowAsync(accountNumber, cancellationToken);

        var transaction = account.Withdraw(amount, clock.GetUtcNow().UtcDateTime);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return transaction.ToDto();
    }

    public async Task<IReadOnlyList<TransactionDto>> GetHistoryAsync(string accountNumber, CancellationToken cancellationToken = default)
    {
        var account = await GetAccountOrThrowAsync(accountNumber, cancellationToken);
        var transactions = await accounts.GetTransactionsAsync(account.Id, cancellationToken);

        return transactions.ToDto().ToList();
    }

    private async Task<Account> GetAccountOrThrowAsync(string accountNumber, CancellationToken cancellationToken) =>
        await accounts.GetByNumberAsync(accountNumber, cancellationToken)
            ?? throw new NotFoundException("Cuenta", accountNumber);

    private async Task<string> GenerateUniqueNumberAsync(CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < MaxNumberAttempts; attempt++)
        {
            var candidate = numberGenerator.Generate();
            if (!await accounts.ExistsByNumberAsync(candidate, cancellationToken))
                return candidate;
        }

        throw new ConflictException("No fue posible generar un número de cuenta único. Intente nuevamente.");
    }
}