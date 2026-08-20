using Mailbox.Shared.Accounts;
using Mailbox.Shared.Identity;

namespace Mailbox.Server.Endpoints.Accounts;

public class GetAccounts(IAccountService accountService) : EndpointWithoutRequest<Result<AccountResponse.GetAccounts>>
{
    public override void Configure()
    {
        Get("/api/accounts");
        Roles(AppRoles.Admin);
    }

    public override Task<Result<AccountResponse.GetAccounts>> ExecuteAsync(CancellationToken ct)
    {
        return accountService.GetAccounts(ct);
    }
}