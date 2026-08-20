namespace Mailbox.Shared.Accounts.Dtos;

public static class AccountDto
{
    public class Simple
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
    }
    
    public class Detailed
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
    }
}