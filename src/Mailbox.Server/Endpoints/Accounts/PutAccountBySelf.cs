using Mailbox.Shared.Accounts;
using Mailbox.Shared.Identity;

namespace Mailbox.Server.Endpoints.Accounts;

public class PutAccountBySelf(IAccountService accountService) : Endpoint<AccountRequest.PutAccount, Result<AccountResponse.PutAccount>>
{
    public override void Configure()
    {
        Put("/api/accounts/self");
        Roles(AppRoles.User);
    }

    public override Task<Result<AccountResponse.PutAccount>> ExecuteAsync(AccountRequest.PutAccount req, CancellationToken ct)
    {
        return accountService.PutAccountBySelf(req,ct);
    }
}