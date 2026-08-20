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