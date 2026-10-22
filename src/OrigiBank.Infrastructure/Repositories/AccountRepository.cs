using Microsoft.EntityFrameworkCore;
using OrigiBank.Application.Abstractions;
using OrigiBank.Domain.Entities;
using OrigiBank.Infrastructure.Persistence;

namespace OrigiBank.Infrastructure.Repositories;

public sealed class AccountRepository(OrigiBankDbContext context) : IAccountRepository
{
    public async Task AddAsync(Account account, CancellationToken cancellationToken = default) =>
        await context.Accounts.AddAsync(account, cancellationToken);

    public Task<Account?> GetByNumberAsync(string accountNumber, CancellationToken cancellationToken = default) =>
        context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber, cancellationToken);

    public Task<bool> ExistsByNumberAsync(string accountNumber, CancellationToken cancellationToken = default) =>
        context.Accounts.AnyAsync(a => a.AccountNumber == accountNumber, cancellationToken);

    public async Task<IReadOnlyList<Transaction>> GetTransactionsAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        return await context.Transactions
            .AsNoTracking()
            .Where(t => t.AccountId == accountId)
            .OrderBy(t => t.TimestampUtc)
            .ThenBy(t => t.Id)
            .ToListAsync(cancellationToken);
    }
}