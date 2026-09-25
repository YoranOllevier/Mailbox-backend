namespace Mailbox.Shared.Mails;

public partial class MailRespone{

    public class PutMail
    {
        public required int Id { get; set; }
    }
}

public partial class MailRequest{

    public class PutMail
    {
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public List<int>? To { get; set; }
        public bool? IsStarred { get; set; }
        public bool? IsDeleted { get; set; }
        public List<string>? Labels { get; set; }

        public class Validator : AbstractValidator<PutMail>
        {
            public Validator()
            {
            }
        }
    }
}