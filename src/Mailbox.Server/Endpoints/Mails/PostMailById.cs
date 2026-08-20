using Mailbox.Shared.Identity;
using Mailbox.Shared.Mails;

namespace Mailbox.Server.Endpoints.Mails;

public class PostMailById(IMailService mailService) : Endpoint<MailRequest.PostMail,Result<MailRespone.PostMail>>
{
    public override void Configure()
    {
        Post("/api/mails/{id:int}");
        Roles(AppRoles.Admin);
    }

    public override Task<Result<MailRespone.PostMail>> ExecuteAsync(MailRequest.PostMail req, CancellationToken ct)
    {
        var id = Route<int>("id");
        return mailService.PostDraftBySelfAsync(req, ct);
    }
}