using Mailbox.Shared.Accounts;
using Mailbox.Shared.Accounts.Dtos;
using Mailbox.Shared.Identity;

namespace Mailbox.Server.Endpoints.Accounts;

public class GetAccountById(IAccountService accountService) : EndpointWithoutRequest<Result<AccountDto.Detailed>>
{
    public override void Configure()
    {
        Get("/api/accounts/{id:int}");
        Roles(AppRoles.Admin);
    }

    public override Task<Result<AccountDto.Detailed>> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<int>("id");
        return accountService.GetAccount(id, ct);
    }
}