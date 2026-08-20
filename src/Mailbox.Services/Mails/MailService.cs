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
}