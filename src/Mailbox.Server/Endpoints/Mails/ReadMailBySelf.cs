using Mailbox.Shared.Identity;
using Mailbox.Shared.Mails;

namespace Mailbox.Server.Endpoints.Mails;

public class ReadMailBySelf(IMailService mailService) : EndpointWithoutRequest<Result<MailRespone.ReadMail>>
{
    public override void Configure()
    {
        Put("/api/mails/read/{mailId:int}/self");
        Roles(AppRoles.User);
    }

    public override Task<Result<MailRespone.ReadMail>> ExecuteAsync(CancellationToken ct)
    {
        var mailId = Route<int>("mailId");
        return mailService.ReadMailBySelfAsync(mailId,ct);
    }
}