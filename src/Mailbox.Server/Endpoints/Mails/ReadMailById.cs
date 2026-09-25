using Mailbox.Shared.Identity;
using Mailbox.Shared.Mails;

namespace Mailbox.Server.Endpoints.Mails;

public class ReadMailById(IMailService mailService) : Endpoint<MailRequest.ReadMail,Result<MailRespone.ReadMail>>
{
    public override void Configure()
    {
        Put("/api/mails/read/{mailId:int}/{accountId:int}");
        Roles(AppRoles.Admin);
    }

    public override Task<Result<MailRespone.ReadMail>> ExecuteAsync(MailRequest.ReadMail req, CancellationToken ct)
    {
        var mailId = Route<int>("mailId");  
        var accountId = Route<int>("accountId");
        return mailService.ReadMailByIdAsync(accountId, mailId, req, ct);
    }
}