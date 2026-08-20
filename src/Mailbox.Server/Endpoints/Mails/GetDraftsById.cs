using Mailbox.Shared.Identity;
using Mailbox.Shared.Mails;

namespace Mailbox.Server.Endpoints.Mails;

public class GetDraftsById(IMailService mailService) : EndpointWithoutRequest<Result<MailRespone.GetMails>>
{
 
 public override void Configure()
 {
  Get("/api/mails/drafts/{id:int}");
  Roles(AppRoles.Admin);
 }

 public override Task<Result<MailRespone.GetMails>> ExecuteAsync(CancellationToken ct)
 {
  var id = Route<int>("id");
  return mailService.GetDraftsByIdAsync(id, ct);
 }
}