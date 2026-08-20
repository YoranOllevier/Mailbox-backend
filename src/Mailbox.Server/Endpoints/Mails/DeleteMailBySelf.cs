using Mailbox.Shared.Identity;
using Mailbox.Shared.Mails;

namespace Mailbox.Server.Endpoints.Mails;

public class DeleteMailBySelf(IMailService mailService) : EndpointWithoutRequest<Result<MailRespone.DeleteMail>>
{
    public override void Configure()
    {
        Delete("/api/mails/{mailId:int}/self");
        Roles(AppRoles.User);
    }

    public override Task<Result<MailRespone.DeleteMail>> ExecuteAsync(CancellationToken ct)
    {
        var mailId = Route<int>("mailId");  
        return mailService.DeleteMailBySelfAsync(mailId,ct);
    }
}