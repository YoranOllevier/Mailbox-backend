using Mailbox.Shared.Mails.Dtos;

namespace Mailbox.Shared.Mails;

public interface IMailService
{
    Task<Result<MailRespone.GetMails>> GetMailsAsync(MailRequest.GetMail filters, CancellationToken ct);
    Task<Result<MailRespone.GetMails>> GetMailsBySelfAsync(MailRequest.GetMail filters, CancellationToken ct);
    Task<Result<MailRespone.GetMails>> GetMailsToSelfAsync(MailRequest.GetMail filters, CancellationToken ct);
    Task<Result<MailRespone.GetMails>> GetMailsForSelfAsync(MailRequest.GetMail filters, CancellationToken ct);
    Task<Result<MailDto.Detailed>> GetMailByIdBySelfAsync(int mailId, CancellationToken ct);
    Task<Result<MailDto.Detailed>> GetMailByIdAsync(int mailId, CancellationToken ct);
    Task<Result<MailRespone.PostMail>> PostByIdAsync(int accountId, MailRequest.PostMail req, CancellationToken ct);
    Task<Result<MailRespone.PostMail>> PostBySelfAsync(MailRequest.PostMail req, CancellationToken ct);
    Task<Result<MailRespone.PutMail>> PutByIdAsync(int accountId, int mailId, MailRequest.PutMail req, CancellationToken ct);
    Task<Result<MailRespone.PutMail>> PutBySelfAsync(int mailId, MailRequest.PutMail req, CancellationToken ct);
    Task<Result<MailRespone.SendMail>> SendMailByIdAsync(int accountId, int mailId, CancellationToken ct);
    Task<Result<MailRespone.SendMail>> SendMailBySelfAsync(int mailId, CancellationToken ct);
    Task<Result<MailRespone.ReadMail>> ReadMailByIdAsync(int accountId, int mailId, MailRequest.ReadMail req, CancellationToken ct);
    Task<Result<MailRespone.ReadMail>> ReadMailBySelfAsync(int mailId, MailRequest.ReadMail req, CancellationToken ct);
    Task<Result<MailRespone.DeleteMail>> DeleteMailByIdAsync(int mailId, CancellationToken ct);
    Task<Result<MailRespone.DeleteMail>> DeleteMailBySelfAsync(int mailId, CancellationToken ct);
}