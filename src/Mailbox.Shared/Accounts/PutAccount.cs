namespace Mailbox.Shared.Accounts;

public partial class AccountResponse
{
    public class PutAccount
    {
        public required int Id { get; set; }
    }
}

public partial class AccountRequest
{
    public class PutAccount
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }

        public class Validator : AbstractValidator<PutAccount>
        {
            public Validator()
            {
                RuleFor(x => x.Name).NotEmpty().When(x => x.Name is not null);
                RuleFor(x => x.Email).EmailAddress().When(x => x.Email is not null);
                RuleFor(x => x.Password).NotEmpty().When(x => x.Password is not null);
            }
        }
    }
}