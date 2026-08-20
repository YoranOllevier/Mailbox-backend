using Mailbox.Domain.Mails;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Mailbox.Persistence.Configurations.Mails;

internal class MailConfiguration : EntityConfiguration<Mail>
{
    public override void Configure(EntityTypeBuilder<Mail> builder)
    {
        base.Configure(builder);

        builder.Property(m => m.Subject).HasMaxLength(255);
        builder.Property(m => m.Body).HasMaxLength(2048);
        builder.Property(m => m.IsSent);
        builder.Property(m => m.IsRead);
        builder.Property(m => m.IsStarred);
        builder.HasOne(m => m.From).WithMany().IsRequired();
        builder.HasMany(m => m.To).WithMany();

        builder.HasMany(m => m.Labels).WithMany();
    }
}