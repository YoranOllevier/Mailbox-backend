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

    public async Task<Result<MailRespone.GetMails>> GetMailsAsync(MailRequest.GetMail filters, CancellationToken ct)
    {
        var account = await dbContext.Accounts.Where(a => a.Id == filters.From).FirstOrDefaultAsync(ct);

        var query = dbContext.Mails
            .Include(m => m.To)
            .Include(m => m.Labels)
            .Where(m => true);
        if (filters.IsSent.HasValue)
            query = query.Where(m => m.IsSent == filters.IsSent);
        if (filters.IsDeleted.HasValue)
            query = query.Where(m => m.IsDeleted == filters.IsDeleted);
        if (filters.IsStarred.HasValue)
            query = query.Where(m => m.IsStarred == filters.IsStarred);
        if(filters.From is not null)
            query = filters.To is not null 
                ? query.Where(m => m.From.Id == filters.From || m.To.Any(a => a.Id == filters.To)) 
                : query.Where(m => m.From.Id == filters.From);
        else if(filters.To is not null)
            query = query.Where(m => m.To.Any(a => a.Id == filters.To));
        if(filters.Subject is not null)
            query = query.Where(m => m.Subject.Contains(filters.Subject));
        if(filters.Body is not null)
            query = query.Where(m => m.Body.Contains(filters.Body));
        if(filters.Include is not null)
            query = query.Where(m => filters.Include.All(s => m.Labels.Any(l => l.Name.ToLower() == s.ToLower())));
        if(filters.Exclude is not null)
            query = query.Where(m => filters.Exclude.All(s => m.Labels.All(l => l.Name.ToLower() != s.ToLower())));
        if(filters.Max.HasValue)
            query = query.Take(filters.Max.Value);
        
        var mails = await query.ToListAsync(ct);

        var dtos = mails.Select(MailToSimpleDto).ToList().AsReadOnly();
        
        return Result.Success(new MailRespone.GetMails{Mails = dtos, Total = mails.Count});
    }

    public async Task<Result<MailRespone.GetMails>> GetMailsToSelfAsync(MailRequest.GetMail filters, CancellationToken ct)
    {
        var user = sessionContextProvider.User;
        if (user is null)
            throw new UnauthorizedAccessException("You are not logged in");

        var account = await dbContext.Accounts.FirstOrDefaultAsync(a => a.UserId == user.GetUserId(), ct);
        if (account is null)
            throw new ArgumentException("Account not found");

        filters.To = account.Id;
        
        return await GetMailsAsync(filters, ct);
    }

    public async Task<Result<MailRespone.GetMails>> GetMailsBySelfAsync(MailRequest.GetMail filters, CancellationToken ct)
    {
        var user = sessionContextProvider.User;
        if (user is null)
            throw new UnauthorizedAccessException("You are not logged in");

        var account = await dbContext.Accounts.FirstOrDefaultAsync(a => a.UserId == user.GetUserId(), ct);
        if (account is null)
            throw new ArgumentException("Account not found");

        filters.From = account.Id;
        
        return await GetMailsAsync(filters, ct);
    }
    
    public async Task<Result<MailRespone.GetMails>> GetMailsForSelfAsync(MailRequest.GetMail filters, CancellationToken ct)
    {
        var user = sessionContextProvider.User;
        if (user is null)
            throw new UnauthorizedAccessException("You are not logged in");

        var account = await dbContext.Accounts.FirstOrDefaultAsync(a => a.UserId == user.GetUserId(), ct);
        if (account is null)
            throw new ArgumentException("Account not found");

        filters.To = account.Id;
        filters.From = account.Id;
        
        return await GetMailsAsync(filters, ct);
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
    
    public async Task<Result<MailRespone.PostMail>> PostByIdAsync(int accountId, MailRequest.PostMail req, CancellationToken ct)
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
            mail.Labels = dbContext.Labels.Where(l => req.Labels.Contains(l.Name) && (l.Owner == null || l.Owner.Id == accountId)).ToHashSet();
        
        await dbContext.Mails.AddAsync(mail,ct);
        
        await dbContext.SaveChangesAsync(ct);
        return Result.Success(new MailRespone.PostMail{Id = mail.Id});
    }

    public async Task<Result<MailRespone.PostMail>> PostBySelfAsync(MailRequest.PostMail req, CancellationToken ct)
    {
        var user = sessionContextProvider.User;
        if (user is null)
            throw new UnauthorizedAccessException("You are not logged in");

        var account = await dbContext.Accounts.FirstOrDefaultAsync(a => a.UserId == user.GetUserId(), ct);
        if (account is null)
            throw new ArgumentException("Account not found");
    
        return await PostByIdAsync(account.Id, req, ct);
    }

    #endregion
    
    #region Put

    public async Task<Result<MailRespone.PutMail>> PutByIdAsync(int accountId, int mailId, MailRequest.PutMail req, CancellationToken ct)
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
        
        if(req.IsDeleted is not null)
            mail.IsDeleted = req.IsDeleted.Value;
        
        if(req.Labels is not null)
            mail.Labels = dbContext.Labels.Where(l => req.Labels.Contains(l.Name) && (l.Owner == null || l.Owner.Id == accountId)).ToHashSet();
        
        dbContext.Mails.Update(mail);
        await dbContext.SaveChangesAsync(ct);
        return Result.Success(new MailRespone.PutMail{Id = mail.Id});
    }

    public async Task<Result<MailRespone.PutMail>> PutBySelfAsync(int mailId, MailRequest.PutMail req, CancellationToken ct)
    {
        var user = sessionContextProvider.User;
        if (user is null)
            throw new UnauthorizedAccessException("You are not logged in");

        var account = await dbContext.Accounts.FirstOrDefaultAsync(a => a.UserId == user.GetUserId(), ct);
        if (account is null)
            throw new ArgumentException("Account not found");
        
        return await PutByIdAsync(account.Id, mailId, req, ct);
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

    public async Task<Result<MailRespone.ReadMail>> ReadMailByIdAsync(int accountId, int mailId, MailRequest.ReadMail req, CancellationToken ct)
    {
        var mail = dbContext.Mails.FirstOrDefault(m => m.Id == mailId && m.To.Any(a => a.Id == accountId));
        if (mail is null)
            throw new ArgumentException($"Mail with ID {mailId} not found");
        
        mail.IsRead = req.isRead;
        
        dbContext.Mails.Update(mail);
        await dbContext.SaveChangesAsync(ct);
        
        return Result.Success(new MailRespone.ReadMail{Id = mail.Id});
    }

    public async Task<Result<MailRespone.ReadMail>> ReadMailBySelfAsync(int mailId, MailRequest.ReadMail req, CancellationToken ct)
    {
        var user = sessionContextProvider.User;
        if (user is null)
            throw new UnauthorizedAccessException("You are not logged in");

        var account = await dbContext.Accounts.FirstOrDefaultAsync(a => a.UserId == user.GetUserId(), ct);
        if (account is null)
            throw new ArgumentException("Account not found");
        
        return await ReadMailByIdAsync(account.Id, mailId, req, ct);
    }

    #endregion

    #region Delete

    public async Task<Result<MailRespone.DeleteMail>> DeleteMailByIdAsync(int mailId, CancellationToken ct)
    {
        var mail = dbContext.Mails.FirstOrDefault(m => m.Id == mailId);
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

        var mail = dbContext.Mails.FirstOrDefault(m => m.Id == mailId && (m.From.Id == account.Id || m.To.Any(a => a.Id == account.Id)));
        if (mail is null)
            throw new ArgumentException($"Mail with ID {mailId} not found");
        
        dbContext.Mails.Remove(mail);
        await dbContext.SaveChangesAsync(ct);
        
        return Result.Success(new MailRespone.DeleteMail{Id = mail.Id});
    }
    
    #endregion
}