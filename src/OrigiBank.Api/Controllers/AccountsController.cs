using Microsoft.AspNetCore.Mvc;
using OrigiBank.Application.Dtos;
using OrigiBank.Application.Services;

namespace OrigiBank.Api.Controllers;

[ApiController]
[Route("api/accounts")]
public class AccountsController(IAccountService service) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<AccountDto>> Create(CreateAccountRequest request, CancellationToken cancellationToken)
    {
        var account = await service.CreateAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, account);
    }

    [HttpGet("{accountNumber}/balance")]
    public async Task<ActionResult<BalanceDto>> GetBalance(string accountNumber, CancellationToken cancellationToken)
        => Ok(await service.GetBalanceAsync(accountNumber, cancellationToken));

    [HttpPost("{accountNumber}/deposits")]
    public async Task<ActionResult<TransactionDto>> Deposit(string accountNumber, AmountRequest request, CancellationToken cancellationToken)
        => Ok(await service.DepositAsync(accountNumber, request.Amount, cancellationToken));

    [HttpPost("{accountNumber}/withdrawals")]
    public async Task<ActionResult<TransactionDto>> Withdraw(string accountNumber, AmountRequest request, CancellationToken cancellationToken)
        => Ok(await service.WithdrawAsync(accountNumber, request.Amount, cancellationToken));

    [HttpGet("{accountNumber}/transactions")]
    public async Task<ActionResult<IReadOnlyList<TransactionDto>>> GetHistory(string accountNumber, CancellationToken cancellationToken)
        => Ok(await service.GetHistoryAsync(accountNumber, cancellationToken));
}