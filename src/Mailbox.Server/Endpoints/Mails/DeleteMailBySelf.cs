using Mailbox.Shared.Identity;
using Mailbox.Shared.Mails;

namespace Mailbox.Server.Endpoints.Mails;

public class DeleteMailBySelf(IMailService mailService) : EndpointWithoutRequest<Result<MailRespone.DeleteMail>>
{
    public override void Configure()
    {
        Delete("api/mails/{id:int}/self");
        Roles(AppRoles.User);
    }

    public override Task<Result<MailRespone.DeleteMail>> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<int>("id");
        return mailService.DeleteMailBySelfAsync(id, ct);
    }
}