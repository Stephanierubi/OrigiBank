using OrigiBank.Application.Dtos;

namespace OrigiBank.Application.Services;

public interface IAccountService
{
    Task<AccountDto> CreateAsync(CreateAccountRequest request, CancellationToken cancellationToken = default);
    Task<BalanceDto> GetBalanceAsync(string accountNumber, CancellationToken cancellationToken = default);
    Task<TransactionDto> DepositAsync(string accountNumber, decimal amount, CancellationToken cancellationToken = default);
    Task<TransactionDto> WithdrawAsync(string accountNumber, decimal amount, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TransactionDto>> GetHistoryAsync(string accountNumber, CancellationToken cancellationToken = default);
}