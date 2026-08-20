using Mailbox.Shared.Identity;
using Mailbox.Shared.Mails;

namespace Mailbox.Server.Endpoints.Mails;

public class DeleteMailById(IMailService mailService) : EndpointWithoutRequest<Result<MailRespone.DeleteMail>>
{
    public override void Configure()
    {
        Delete("/api/mails/{mailId:int}/{accountId:int}");
        Roles(AppRoles.Admin);
    }

    public override Task<Result<MailRespone.DeleteMail>> ExecuteAsync(CancellationToken ct)
    {
        var mailId = Route<int>("mailId");
        var accountId = Route<int>("accountId");
        return mailService.DeleteMailByIdAsync(accountId,mailId,ct);
    }
}