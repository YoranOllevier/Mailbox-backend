using Mailbox.Shared.Accounts.Dtos;

namespace Mailbox.Shared.Accounts;

public partial class AccountResponse
{
    public class GetAccounts
    {
        public required IReadOnlyList<AccountDto.Simple> Accounts { get; set; }
    }
}