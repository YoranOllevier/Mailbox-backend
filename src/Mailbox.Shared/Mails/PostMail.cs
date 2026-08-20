namespace Mailbox.Shared.Mails;

public partial class MailRespone{

    public class PostMail
    {
        public required int Id { get; set; }
    }
}

public partial class MailRequest{

    public class PostMail
    {
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public List<int>? To { get; set; }
        public bool? IsStarred { get; set; }
        public List<int>? Labels { get; set; }

        public class Validator : AbstractValidator<PostMail>
        {
            public Validator()
            {
                RuleFor(x => x.Subject).NotEmpty().When(x => x.Body is not null);
                RuleFor(x => x.Body).NotEmpty().When(x => x.Body is not null);
                RuleFor(x => x.To).NotEmpty().When(x => x.To is not null);
                RuleFor(x=>x.Labels).NotEmpty().When(x => x.Labels is not null);
            }
        }
    }
}