using Mailbox.Shared.Identity;
using Mailbox.Shared.Mails;

namespace Mailbox.Server.Endpoints.Mails;

public class DeleteMailById(IMailService mailService) : EndpointWithoutRequest<Result<MailRespone.DeleteMail>>
{
    public override void Configure()
    {
        Delete("api/mails/{id:int}");
        Roles(AppRoles.Admin);
    }

    public override Task<Result<MailRespone.DeleteMail>> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<int>("id");
        return mailService.DeleteMailByIdAsync(id, ct);
    }
}