using Mailbox.Shared.Mails.Dtos;

namespace Mailbox.Shared.Mails;

public partial class MailRespone
{
    public class GetMails
    {
        public required IReadOnlyList<MailDto.Simple> Mails {get; set;}

        public required int Total {get; set;}
    }
}

public partial class MailRequest{

    public class GetMail
    {
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public int? From { get; set; }
        public int? To { get; set; }
        public bool? IsSent { get; set; }
        public bool? IsStarred { get; set; }
        public bool? IsDeleted { get; set; }
        public List<string>? Include { get; set; }
        public List<string>? Exclude { get; set; }
        public int? Max { get; set; }

        public class Validator : AbstractValidator<GetMail>
        {
            public Validator()
            {
            }
        }
    }
}