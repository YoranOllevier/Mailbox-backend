using Mailbox.Shared.Identity;
using Mailbox.Shared.Mails;

namespace Mailbox.Server.Endpoints.Mails;

public class SendMailById(IMailService mailService) : EndpointWithoutRequest<Result<MailRespone.SendMail>>
{
    public override void Configure()
    {
        Put("/api/mails/send/{mailId:int}/{accountId:int}");
        Roles(AppRoles.Admin);
    }

    public override Task<Result<MailRespone.SendMail>> ExecuteAsync(CancellationToken ct)
    {
        var mailId = Route<int>("mailId");
        var accountId = Route<int>("accountId");
        return mailService.SendMailByIdAsync(accountId,mailId,ct);
    }
}