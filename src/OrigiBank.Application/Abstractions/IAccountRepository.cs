using OrigiBank.Domain.Entities;

namespace OrigiBank.Application.Abstractions;

public interface IAccountRepository
{
    Task AddAsync(Account account, CancellationToken cancellationToken = default);
    Task<Account?> GetByNumberAsync(string accountNumber, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNumberAsync(string accountNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Transaction>> GetTransactionsAsync(Guid accountId, CancellationToken cancellationToken = default);
}