using Mailbox.Shared.Identity;
using Mailbox.Shared.Mails;

namespace Mailbox.Server.Endpoints.Mails;

public class PutMailBySelf(IMailService mailService) : Endpoint<MailRequest.PutMail,Result<MailRespone.PutMail>>
{
    public override void Configure()
    {
        Put("/api/mails/{mailId:int}/self");
        Roles(AppRoles.User);
    }

    public override Task<Result<MailRespone.PutMail>> ExecuteAsync(MailRequest.PutMail req, CancellationToken ct)
    {
        var mailId = Route<int>("mailId");
        return mailService.PutBySelfAsync(mailId, req, ct);
    }
}