using Mailbox.Services.Identity;
using Mailbox.Shared.Accounts;
using Mailbox.Shared.Accounts.Dtos;
using Mailbox.Shared.Identity;

namespace Mailbox.Server.Endpoints.Accounts;

public class GetAccountBySelf(IAccountService accountService) : EndpointWithoutRequest<Result<AccountDto.Detailed>>
{
    public override void Configure()
    {
        Get("/api/accounts/self");
        Roles(AppRoles.User);
    }

    public override Task<Result<AccountDto.Detailed>> ExecuteAsync(CancellationToken ct)
    {
        return accountService.GetAccountBySelf(ct);
    }
}