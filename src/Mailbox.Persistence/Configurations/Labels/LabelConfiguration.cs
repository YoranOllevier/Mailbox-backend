using Mailbox.Domain.Labels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mailbox.Persistence.Configurations.Labels;

internal class LabelConfiguration : EntityConfiguration<Label>
{
    public override void Configure(EntityTypeBuilder<Label> builder)
    {
        base.Configure(builder);
        
        builder.Property(l => l.Name).HasMaxLength(50).IsRequired();
        builder.HasOne(l => l.Owner).WithMany().HasForeignKey("OwnerId").IsRequired(false);
        
        builder.HasIndex("OwnerId", nameof(Label.Name))
            .IsUnique();
    }
}