using Mailbox.Shared.Accounts.Dtos;
using Mailbox.Shared.Labels.Dtos;

namespace Mailbox.Shared.Mails.Dtos;

public static class MailDto
{
    public class Simple
    {
        public required int Id { get; set; }
        public required AccountDto.Simple From {get; set;}
        public required string Subject {get; set;}
        public required string Body {get; set;}
        public required DateTime SentOn {get; set;}
        public required bool IsRead {get; set;}
        public required bool IsSent {get; set;}
        public required bool IsStarred {get; set;}
        public required ICollection<LabelDto.Simple> Labels {get; set;}
    }
    
    public class Detailed
    {
        public required int Id { get; set; }
        public required AccountDto.Detailed From {get; set;}
        public required ICollection<AccountDto.Detailed> To {get; set;}
        public required string Subject {get; set;}
        public required string Body {get; set;}
        public required DateTime SentOn {get; set;}
        public required bool IsRead {get; set;}
        public required bool IsSent {get; set;}
        public required bool IsStarred {get; set;}
        public required ICollection<LabelDto.Simple> Labels {get; set;}
    }
}