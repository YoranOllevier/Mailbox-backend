using Mailbox.Shared.Identity;
using Mailbox.Shared.Mails;

namespace Mailbox.Server.Endpoints.Mails;

public class GetMailsToSelf(IMailService mailService) : Endpoint<MailRequest.GetMail, Result<MailRespone.GetMails>>
{
 
 public override void Configure()
 {
  Get("/api/mails/inbox");
  Roles(AppRoles.User);
 }

 public override Task<Result<MailRespone.GetMails>> ExecuteAsync(MailRequest.GetMail req, CancellationToken ct)
 {
  return mailService.GetMailsToSelfAsync(req,ct);
 }
}