using Mailbox.Shared.Identity;
using Mailbox.Shared.Mails;

namespace Mailbox.Server.Endpoints.Mails;

public class ReadMailBySelf(IMailService mailService) : Endpoint<MailRequest.ReadMail, Result<MailRespone.ReadMail>>
{
    public override void Configure()
    {
        Put("/api/mails/read/{mailId:int}/self");
        Roles(AppRoles.User);
    }

    public override Task<Result<MailRespone.ReadMail>> ExecuteAsync(MailRequest.ReadMail req, CancellationToken ct)
    {
        var mailId = Route<int>("mailId");
        return mailService.ReadMailBySelfAsync(mailId, req, ct);
    }
}