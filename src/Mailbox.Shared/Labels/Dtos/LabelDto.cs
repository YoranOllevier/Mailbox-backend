using Mailbox.Shared.Accounts.Dtos;

namespace Mailbox.Shared.Labels.Dtos;

public static class LabelDto
{
    public class Simple
    {
        public required string Name { get; set; }
    }
    
    public class Detailed
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required AccountDto.Simple? Owner { get; set; }
    }
}