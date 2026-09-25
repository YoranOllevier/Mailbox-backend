using Mailbox.Domain.Accounts;
using Mailbox.Domain.Labels;
using Mailbox.Domain.Mails;
using Mailbox.Shared.Accounts.Dtos;
using Mailbox.Shared.Labels.Dtos;
using Mailbox.Shared.Mails.Dtos;

namespace Mailbox.Services.Mappers;

public static class Mappers
{
    #region Account

    public static AccountDto.Simple AccountToSimpleDto(Account account)
    {
        return new AccountDto.Simple
        {
            Id = account.Id,
            Name = account.Name,
        };
    }

    public static AccountDto.Detailed AccountToDetailedDto(Account account)
    {
        return new AccountDto.Detailed
        {
            Id = account.Id,
            Name = account.Name,
            Email = account.Email.Address
        };
    }

    #endregion
    
    #region Label

    public static LabelDto.Simple LabelToSimpleDto(Label label)
    {
        return new LabelDto.Simple
        {
            Name = label.Name
        };
    }

    public static LabelDto.Detailed LabelToDetailedDto(Label label)
    {
        return new LabelDto.Detailed
        {
            Id = label.Id,
            Name = label.Name,
            Owner = label.Owner is null ? null : AccountToSimpleDto(label.Owner),
        };
    }
    
    #endregion

    #region Mail

    public static MailDto.Simple MailToSimpleDto(Mail mail)
    {
        return new MailDto.Simple
        {
            Id = mail.Id,
            From = AccountToSimpleDto(mail.From),
            Subject = mail.Subject,
            Body = mail.Body,
            SentOn = mail.SentOn,
            IsRead = mail.IsRead,
            IsSent = mail.IsSent,
            IsStarred = mail.IsStarred,
            IsDeleted = mail.IsDeleted,
            Labels = mail.Labels.Select(LabelToSimpleDto).ToArray(),
        };
    }

    public static MailDto.Detailed MailToDetailedDto(Mail mail)
    {
        return new MailDto.Detailed
        {
            Id = mail.Id,
            To = mail.To.Select(AccountToDetailedDto).ToArray(),
            From = AccountToDetailedDto(mail.From),
            Subject = mail.Subject,
            Body = mail.Body,
            SentOn = mail.SentOn,
            IsRead = mail.IsRead,
            IsSent = mail.IsSent,
            IsStarred = mail.IsStarred,
            IsDeleted = mail.IsDeleted,
            Labels = mail.Labels.Select(LabelToSimpleDto).ToArray(),
        };
    }

    #endregion
}