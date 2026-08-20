using Mailbox.Shared.Identity;
using Mailbox.Shared.Mails;

namespace Mailbox.Server.Endpoints.Mails;

public class ReadMailById(IMailService mailService) : EndpointWithoutRequest<Result<MailRespone.ReadMail>>
{
    public override void Configure()
    {
        Put("/api/mails/read/{mailId:int}/{accountId:int}");
        Roles(AppRoles.Admin);
    }

    public override Task<Result<MailRespone.ReadMail>> ExecuteAsync(CancellationToken ct)
    {
        var mailId = Route<int>("mailId");  
        var accountId = Route<int>("accountId");
        return mailService.ReadMailByIdAsync(accountId, mailId,ct);
    }
}