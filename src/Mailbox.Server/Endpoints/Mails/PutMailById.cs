using Mailbox.Shared.Identity;
using Mailbox.Shared.Mails;

namespace Mailbox.Server.Endpoints.Mails;

public class PutMailById(IMailService mailService) : Endpoint<MailRequest.PutMail,Result<MailRespone.PutMail>>
{
    public override void Configure()
    {
        Put("/api/mails/{mailId:int}/{accountId:int}");
        Roles(AppRoles.Admin);
    }

    public override Task<Result<MailRespone.PutMail>> ExecuteAsync(MailRequest.PutMail req, CancellationToken ct)
    {
        var accountId = Route<int>("accountId");
        var mailId = Route<int>("mailId");
        return mailService.PutDraftByIdAsync(accountId, mailId, req, ct);
    }
}