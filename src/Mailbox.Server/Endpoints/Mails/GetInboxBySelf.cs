using Mailbox.Shared.Identity;
using Mailbox.Shared.Mails;

namespace Mailbox.Server.Endpoints.Mails;

public class GetInboxBySelf(IMailService mailService) : EndpointWithoutRequest<Result<MailRespone.GetMails>>
{
 
 public override void Configure()
 {
  Get("/api/mails/inbox/self");
  Roles(AppRoles.User);
 }

 public override Task<Result<MailRespone.GetMails>> ExecuteAsync(CancellationToken ct)
 {
  return mailService.GetInboxBySelfAsync(ct);
 }
}