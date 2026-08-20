using Mailbox.Shared.Identity;
using Mailbox.Shared.Mails;

namespace Mailbox.Server.Endpoints.Mails;

public class GetAllBySelf(IMailService mailService) : EndpointWithoutRequest<Result<MailRespone.GetMails>>
{
 
 public override void Configure()
 {
  Get("/api/mails/all/self");
  Roles(AppRoles.User);
 }

 public override Task<Result<MailRespone.GetMails>> ExecuteAsync(CancellationToken ct)
 {
  return mailService.GetAllBySelfAsync(ct);
 }
}