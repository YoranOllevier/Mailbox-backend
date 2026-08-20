using Mailbox.Shared.Identity;
using Mailbox.Shared.Mails;

namespace Mailbox.Server.Endpoints.Mails;

public class SendMailBySelf(IMailService mailService) : EndpointWithoutRequest<Result<MailRespone.SendMail>>
{
    public override void Configure()
    {
        Put("/api/mails/send/{mailId:int}/self");
        Roles(AppRoles.User);
    }

    public override Task<Result<MailRespone.SendMail>> ExecuteAsync(CancellationToken ct)
    {
        var mailId = Route<int>("mailId");
        return mailService.SendMailBySelfAsync(mailId,ct);
    }
}