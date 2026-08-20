using Mailbox.Shared.Accounts.Dtos;

namespace Mailbox.Shared.Accounts;

public interface IAccountService
{
    Task<Result<AccountDto.Detailed>> GetAccount(int id, CancellationToken ct);
    Task<Result<AccountDto.Detailed>> GetAccountBySelf(CancellationToken ct);
    Task<Result<AccountResponse.GetAccounts>> GetAccounts(CancellationToken ct);
    Task<Result<AccountResponse.PutAccount>> PutAccountById(int id, AccountRequest.PutAccount req, CancellationToken ct);
    Task<Result<AccountResponse.PutAccount>> PutAccountBySelf(AccountRequest.PutAccount req, CancellationToken ct);
    Task<Result<AccountResponse.DeleteAccount>> DeleteAccount(int id, CancellationToken ct);
}