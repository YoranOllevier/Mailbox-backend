using Mailbox.Shared.Identity;
using Mailbox.Shared.Mails;

namespace Mailbox.Server.Endpoints.Mails;

public class GetMailsBySelf(IMailService mailService) : Endpoint<MailRequest.GetMail, Result<MailRespone.GetMails>>
{
 
 public override void Configure()
 {
  Get("/api/mails/outbox");
  Roles(AppRoles.User);
 }

 public override Task<Result<MailRespone.GetMails>> ExecuteAsync(MailRequest.GetMail req, CancellationToken ct)
 {
  return mailService.GetMailsBySelfAsync(req,ct);
 }
}