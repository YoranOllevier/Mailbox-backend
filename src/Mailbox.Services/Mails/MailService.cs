using Mailbox.Domain.Accounts;
using Mailbox.Domain.Mails;
using Mailbox.Persistence;
using Mailbox.Services.Identity;
using Mailbox.Shared.Identity;
using Mailbox.Shared.Mails;
using Mailbox.Shared.Mails.Dtos;
using Microsoft.EntityFrameworkCore;
using static Mailbox.Services.Mappers.Mappers;

namespace Mailbox.Services.Mails;

public class MailService(ApplicationDbContext dbContext, ISessionContextProvider sessionContextProvider) : IMailService
{
    #region Get

    public async Task<Result<MailRespone.GetMails>> GetInboxByIdAsync(int accountId, CancellationToken ct)
    {
        var account = await dbContext.Accounts.Where(a => a.Id == accountId).FirstOrDefaultAsync(ct);
        if (account is null)
            throw new ArgumentException($"Account with ID {accountId} not found");
        
        var mails = await dbContext.Mails
            .Include(m => m.To)
            .Include(m => m.Labels)
            .Where(m => m.To.Any(a => a.Id == account.Id) && m.IsSent).ToListAsync(ct);

        var dtos = mails.Select(MailToSimpleDto).ToList().AsReadOnly();
        
        return Result.Success(new MailRespone.GetMails{Mails = dtos, Total = mails.Count});
    }

    public async Task<Result<MailRespone.GetMails>> GetInboxBySelfAsync(CancellationToken ct)
    {
        var user = sessionContextProvider.User;
        if (user is null)
            throw new UnauthorizedAccessException("You are not logged in");

        var account = await dbContext.Accounts.FirstOrDefaultAsync(a => a.UserId == user.GetUserId(), ct);
        if (account is null)
            throw new ArgumentException("Account not found");
        
        return await GetInboxByIdAsync(account.Id, ct);
    }

    public async Task<Result<MailRespone.GetMails>> GetAllByIdAsync(int accountId, CancellationToken ct)
    {
        var account = await dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == accountId, ct);
        if (account is null)
            throw new ArgumentException($"Account with ID {accountId} not found");
        
        var mails = await dbContext.Mails
            .Include(m => m.To)
            .Include(m => m.Labels)
            .Where(m => m.To.Any(a => a.Id == account.Id) || m.From.Id == account.Id).ToListAsync(ct);

        var dtos = mails.Select(MailToSimpleDto).ToList().AsReadOnly();
        
        return Result.Success(new MailRespone.GetMails{Mails = dtos, Total = mails.Count});
    }

    public async Task<Result<MailRespone.GetMails>> GetAllBySelfAsync(CancellationToken ct)
    {
        var user = sessionContextProvider.User;
        if (user is null)
            throw new UnauthorizedAccessException("You are not logged in");

        var account = await dbContext.Accounts.FirstOrDefaultAsync(a => a.UserId == user.GetUserId(), ct);
        if (account is null)
            throw new ArgumentException("Account not found");
        
        return await GetAllByIdAsync(account.Id, ct);
    }

    public async Task<Result<MailRespone.GetMails>> GetSentByIdAsync(int accountId, CancellationToken ct)
    {
        var account = await dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == accountId, ct);
        if (account is null)
            throw new ArgumentException($"Account with ID {accountId} not found");
        
        var mails = await dbContext.Mails
            .Include(m => m.To)
            .Include(m => m.Labels)
            .Where(m => m.From.Id == account.Id && m.IsSent).ToListAsync(ct);

        var dtos = mails.Select(MailToSimpleDto).ToList().AsReadOnly();
        
        return Result.Success(new MailRespone.GetMails{Mails = dtos, Total = mails.Count});
    }

    public async Task<Result<MailRespone.GetMails>> GetSentBySelfAsync(CancellationToken ct)
    {
        var user = sessionContextProvider.User;
        if (user is null)
            throw new UnauthorizedAccessException("You are not logged in");

        var account = await dbContext.Accounts.FirstOrDefaultAsync(a => a.UserId == user.GetUserId(), ct);
        if (account is null)
            throw new ArgumentException("Account not found");

        return await GetSentByIdAsync(account.Id, ct);
    }
    
    
    public async Task<Result<MailRespone.GetMails>> GetDraftsByIdAsync(int accountId, CancellationToken ct)
    {
        var account = await dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == accountId, ct);
        if (account is null)
            throw new ArgumentException($"Account with ID {accountId} not found");
        
        var mails = await dbContext.Mails
            .Include(m => m.To)
            .Include(m => m.Labels)
            .Where(m => m.From.Id == account.Id && !m.IsSent).ToListAsync(ct);

        var dtos = mails.Select(MailToSimpleDto).ToList().AsReadOnly();
        
        return Result.Success(new MailRespone.GetMails{Mails = dtos, Total = mails.Count});
    }

    public async Task<Result<MailRespone.GetMails>> GetDraftsBySelfAsync(CancellationToken ct)
    {
        var user = sessionContextProvider.User;
        if (user is null)
            throw new UnauthorizedAccessException("You are not logged in");

        var account = await dbContext.Accounts.FirstOrDefaultAsync(a => a.UserId == user.GetUserId(), ct);
        if (account is null)
            throw new ArgumentException("Account not found");

        return await GetDraftsByIdAsync(account.Id, ct);
    }

    public async Task<Result<MailDto.Detailed>> GetMailByIdAsync(int mailId, CancellationToken ct)
    {
        var mail = await dbContext.Mails
            .Include(m => m.To)
            .Include(m => m.Labels)
            .FirstOrDefaultAsync(m => m.Id == mailId, ct);
        if (mail is null)
            throw new ArgumentException($"Mail with ID {mailId} not found");
        
        return Result.Success(MailToDetailedDto(mail));
    }

    public async Task<Result<MailDto.Detailed>> GetMailByIdBySelfAsync(int mailId, CancellationToken ct)
    {
        var user = sessionContextProvider.User;
        if (user is null)
            throw new UnauthorizedAccessException("You are not logged in");
        
        var account = await dbContext.Accounts.FirstOrDefaultAsync(a => a.UserId == user.GetUserId(), ct);
        if (account is null)
            throw new ArgumentException("Account not found");
        
        var mail = await dbContext.Mails
            .Include(m => m.To)
            .Include(m => m.Labels)
            .FirstOrDefaultAsync(m => m.Id == mailId && (m.From.Id == account.Id && m.To.Any(a => a.Id == account.Id)), ct);
        if (mail is null)
            throw new ArgumentException($"Mail with ID {mailId} not found");
        
        return Result.Success(MailToDetailedDto(mail));
    }
    
    #endregion

    #region Post
    
    public async Task<Result<MailRespone.PostMail>> PostDraftByIdAsync(int accountId, MailRequest.PostMail req, CancellationToken ct)
    {
        var from = await dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == accountId, ct);
        
        if(from is null)
            throw new ArgumentException($"Account with ID {accountId} not found");

        ISet<Account>? to = null;
        
        if(req.To is not null)
            to = await dbContext.Accounts.Where(a => req.To.Contains(a.Id)).ToHashSetAsync(ct);

        var mail = new Mail(from, to, req.Subject, req.Body);

        if (req.IsStarred is not null)
            mail.IsStarred = req.IsStarred.Value;
        
        if(req.Labels is not null)
            mail.Labels = dbContext.Labels.Where(l => req.Labels.Contains(l.Id)).ToHashSet();
        
        await dbContext.Mails.AddAsync(mail,ct);
        
        await dbContext.SaveChangesAsync(ct);
        return Result.Success(new MailRespone.PostMail{Id = mail.Id});
    }

    public async Task<Result<MailRespone.PostMail>> PostDraftBySelfAsync(MailRequest.PostMail req, CancellationToken ct)
    {
        var user = sessionContextProvider.User;
        if (user is null)
            throw new UnauthorizedAccessException("You are not logged in");

        var account = await dbContext.Accounts.FirstOrDefaultAsync(a => a.UserId == user.GetUserId(), ct);
        if (account is null)
            throw new ArgumentException("Account not found");
    
        return await PostDraftByIdAsync(account.Id, req, ct);
    }

    #endregion
    
    #region Put

    public async Task<Result<MailRespone.PutMail>> PutDraftByIdAsync(int accountId, int mailId, MailRequest.PutMail req, CancellationToken ct)
    {
        var from = await dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == accountId, ct);
        
        if(from is null)
            throw new ArgumentException($"Account with ID {accountId} not found");

        var mail = await dbContext.Mails
            .Include(a => a.To)
            .Include(a => a.Labels)
            .FirstOrDefaultAsync(m => m.Id == mailId && from.Id == accountId, ct);
        
        if (mail is null)
            throw new ArgumentException($"Mail with ID {mailId} not found");
        
        if(req.To is not null)
            mail.To = await dbContext.Accounts.Where(a => req.To.Contains(a.Id)).ToHashSetAsync(ct);
        
        if(req.Subject is not null)
            mail.Subject = req.Subject;
        
        if(req.Body is not null)
            mail.Body = req.Body;
        
        if (req.IsStarred is not null)
            mail.IsStarred = req.IsStarred.Value;
        
        if(req.Labels is not null)
            mail.Labels = dbContext.Labels.Where(l => req.Labels.Contains(l.Id)).ToHashSet();
        
        dbContext.Mails.Update(mail);
        await dbContext.SaveChangesAsync(ct);
        return Result.Success(new MailRespone.PutMail{Id = mail.Id});
    }

    public async Task<Result<MailRespone.PutMail>> PutDraftBySelfAsync(int mailId, MailRequest.PutMail req, CancellationToken ct)
    {
        var user = sessionContextProvider.User;
        if (user is null)
            throw new UnauthorizedAccessException("You are not logged in");

        var account = await dbContext.Accounts.FirstOrDefaultAsync(a => a.UserId == user.GetUserId(), ct);
        if (account is null)
            throw new ArgumentException("Account not found");
        
        return await PutDraftByIdAsync(account.Id, mailId, req, ct);
    }
    
    public async Task<Result<MailRespone.SendMail>> SendMailByIdAsync(int accountId, int mailId, CancellationToken ct)
    {
        var mail = dbContext.Mails.FirstOrDefault(a => a.Id == mailId);
        if (mail is null)
            throw new ArgumentException($"Mail with ID {mailId} not found");

        mail.IsSent = true;
        mail.SentOn = DateTime.Now;

        dbContext.Mails.Update(mail);
        await dbContext.SaveChangesAsync(ct);
        
        return Result.Success(new MailRespone.SendMail{Id = mail.Id});
    }

    public async Task<Result<MailRespone.SendMail>> SendMailBySelfAsync(int mailId, CancellationToken ct)
    {
        var user = sessionContextProvider.User;
        if (user is null)
            throw new UnauthorizedAccessException("You are not logged in");

        var account = await dbContext.Accounts.FirstOrDefaultAsync(a => a.UserId == user.GetUserId(), ct);
        if (account is null)
            throw new ArgumentException("Account not found");
        
        return await SendMailByIdAsync(account.Id, mailId, ct);
    }

    public async Task<Result<MailRespone.ReadMail>> ReadMailByIdAsync(int accountId, int mailId, CancellationToken ct)
    {
        var mail = dbContext.Mails.FirstOrDefault(m => m.Id == mailId && m.To.Any(a => a.Id == accountId));
        if (mail is null)
            throw new ArgumentException($"Mail with ID {mailId} not found");
        
        mail.IsRead = true;
        
        dbContext.Mails.Update(mail);
        await dbContext.SaveChangesAsync(ct);
        
        return Result.Success(new MailRespone.ReadMail{Id = mail.Id});
    }

    public async Task<Result<MailRespone.ReadMail>> ReadMailBySelfAsync(int mailId, CancellationToken ct)
    {
        var user = sessionContextProvider.User;
        if (user is null)
            throw new UnauthorizedAccessException("You are not logged in");

        var account = await dbContext.Accounts.FirstOrDefaultAsync(a => a.UserId == user.GetUserId(), ct);
        if (account is null)
            throw new ArgumentException("Account not found");
        
        return await ReadMailByIdAsync(account.Id, mailId, ct);
    }

    #endregion

    #region Delete

    public async Task<Result<MailRespone.DeleteMail>> DeleteMailByIdAsync(int accountId, int mailId, CancellationToken ct)
    {
        var mail = dbContext.Mails.FirstOrDefault(m => m.Id == mailId && (m.From.Id == accountId || m.To.Any(a => a.Id == accountId)));
        if (mail is null)
            throw new ArgumentException($"Mail with ID {mailId} not found");
        
        dbContext.Mails.Remove(mail);
        await dbContext.SaveChangesAsync(ct);
        
        return Result.Success(new MailRespone.DeleteMail{Id = mail.Id});
    }

    public async Task<Result<MailRespone.DeleteMail>> DeleteMailBySelfAsync(int mailId, CancellationToken ct)
    {
        var user = sessionContextProvider.User;
        if (user is null)
            throw new UnauthorizedAccessException("You are not logged in");

        var account = await dbContext.Accounts.FirstOrDefaultAsync(a => a.UserId == user.GetUserId(), ct);
        if (account is null)
            throw new ArgumentException("Account not found");

        return await DeleteMailByIdAsync(account.Id, mailId, ct);
    }
    
    #endregion
}