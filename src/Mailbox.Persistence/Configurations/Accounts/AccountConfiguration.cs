using System.Net.Mail;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mailbox.Domain.Accounts;

namespace Mailbox.Persistence.Configurations.Accounts;

internal class AccountConfiguration : EntityConfiguration<Account>
{
    public override void Configure(EntityTypeBuilder<Account> builder)
    {
        base.Configure(builder);

        builder.Property(a => a.UserId).IsRequired();
        builder.Property(a => a.Name).HasMaxLength(50).IsRequired();
        builder.Property(r => r.Email).HasConversion(m => m.Address, value => new MailAddress(value));
    }
}