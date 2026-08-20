using Mailbox.Shared.Identity;
using Mailbox.Shared.Mails;
using Mailbox.Shared.Mails.Dtos;

namespace Mailbox.Server.Endpoints.Mails;

public class GetMailByIdBySelf(IMailService mailService) : EndpointWithoutRequest<Result<MailDto.Detailed>>
{
 
 public override void Configure()
 {
  Get("/api/mails/{id:int}/self");
  Roles(AppRoles.User);
 }

 public override Task<Result<MailDto.Detailed>> ExecuteAsync(CancellationToken ct)
 {
  var id = Route<int>("id");
  return mailService.GetMailByIdBySelfAsync(id, ct);
 }
}