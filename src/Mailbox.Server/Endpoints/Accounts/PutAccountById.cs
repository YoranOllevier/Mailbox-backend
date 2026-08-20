using Mailbox.Shared.Accounts;
using Mailbox.Shared.Identity;

namespace Mailbox.Server.Endpoints.Accounts;

public class PutAccountById(IAccountService accountService) : Endpoint<AccountRequest.PutAccount, Result<AccountResponse.PutAccount>>
{
    public override void Configure()
    {
        Put("/api/accounts/{id:int}");
        Roles(AppRoles.Admin);
    }

    public override Task<Result<AccountResponse.PutAccount>> ExecuteAsync(AccountRequest.PutAccount req, CancellationToken ct)
    {
        var id = Route<int>("id");
        return accountService.PutAccountById(id, req,ct);
    }
}