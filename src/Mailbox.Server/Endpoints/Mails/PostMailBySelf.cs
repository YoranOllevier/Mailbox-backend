using Mailbox.Shared.Identity;
using Mailbox.Shared.Mails;

namespace Mailbox.Server.Endpoints.Mails;

public class PostMailBySelf(IMailService mailService) : Endpoint<MailRequest.PostMail,Result<MailRespone.PostMail>>
{
    public override void Configure()
    {
        Post("/api/mails/self");
        Roles(AppRoles.User);
    }

    public override Task<Result<MailRespone.PostMail>> ExecuteAsync(MailRequest.PostMail req, CancellationToken ct)
    {
        return mailService.PostDraftBySelfAsync(req, ct);
    }
}