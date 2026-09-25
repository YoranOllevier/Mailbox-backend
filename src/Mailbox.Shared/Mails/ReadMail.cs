namespace Mailbox.Shared.Mails;

public partial class MailRespone
{
    public class ReadMail
    {
        public required int Id { get; set; }
    }
}

public partial class MailRequest
{
    public class ReadMail
    {
        public bool isRead { get; set; }
    
        public class Validator : AbstractValidator<ReadMail>
        {
            public Validator()
            {
            }
        }
    }
}