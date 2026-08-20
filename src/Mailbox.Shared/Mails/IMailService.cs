using Mailbox.Shared.Mails.Dtos;

namespace Mailbox.Shared.Mails;

public interface IMailService
{
    Task<Result<MailRespone.GetMails>> GetInboxByIdAsync(int accountId, CancellationToken ct);
    Task<Result<MailRespone.GetMails>> GetInboxBySelfAsync(CancellationToken ct);
    Task<Result<MailRespone.GetMails>> GetAllByIdAsync(int accountId, CancellationToken ct);
    Task<Result<MailRespone.GetMails>> GetAllBySelfAsync(CancellationToken ct);
    Task<Result<MailRespone.GetMails>> GetSentByIdAsync(int accountId, CancellationToken ct);
    Task<Result<MailRespone.GetMails>> GetSentBySelfAsync(CancellationToken ct);
    Task<Result<MailRespone.GetMails>> GetDraftsByIdAsync(int accountId, CancellationToken ct);
    Task<Result<MailRespone.GetMails>> GetDraftsBySelfAsync(CancellationToken ct);
    Task<Result<MailDto.Detailed>> GetMailByIdBySelfAsync(int mailId, CancellationToken ct);
    Task<Result<MailDto.Detailed>> GetMailByIdAsync(int mailId, CancellationToken ct);
    Task<Result<MailRespone.PostMail>> PostDraftByIdAsync(int accountId, MailRequest.PostMail req, CancellationToken ct);
    Task<Result<MailRespone.PostMail>> PostDraftBySelfAsync(MailRequest.PostMail req, CancellationToken ct);
    Task<Result<MailRespone.PutMail>> PutDraftByIdAsync(int accountId, int mailId, MailRequest.PutMail req, CancellationToken ct);
    Task<Result<MailRespone.PutMail>> PutDraftBySelfAsync(int mailId, MailRequest.PutMail req, CancellationToken ct);
    Task<Result<MailRespone.SendMail>> SendMailByIdAsync(int accountId, int mailId, CancellationToken ct);
    Task<Result<MailRespone.SendMail>> SendMailBySelfAsync(int mailId, CancellationToken ct);
    Task<Result<MailRespone.ReadMail>> ReadMailByIdAsync(int accountId, int mailId, CancellationToken ct);
    Task<Result<MailRespone.ReadMail>> ReadMailBySelfAsync(int mailId, CancellationToken ct);
    Task<Result<MailRespone.DeleteMail>> DeleteMailByIdAsync(int accountId, int mailId, CancellationToken ct);
    Task<Result<MailRespone.DeleteMail>> DeleteMailBySelfAsync(int mailId, CancellationToken ct);
}