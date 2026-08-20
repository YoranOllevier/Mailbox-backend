using EntityFrameworkCore.Triggered;
using Mailbox.Domain.Labels;
using Microsoft.EntityFrameworkCore;

namespace Mailbox.Persistence.Triggers;

public class LabelBeforeSaveTrigger(ApplicationDbContext dbContext) : IBeforeSaveTrigger<Label>
{
    public async Task BeforeSave(ITriggerContext<Label> context, CancellationToken cancellationToken)
    {
        if (context.ChangeType == ChangeType.Added)
        {
            var label = context.Entity;
            var duplicate = dbContext.Labels.Any(l => l.Name == label.Name && l.Owner == label.Owner);

            if (duplicate)
                throw new InvalidOperationException($"Label with name {label.Name} already exists for owner {label.Owner?.Name ?? "ALL"}");
        }

        if (context.ChangeType == ChangeType.Modified)
        {
            var label = context.Entity;
            var duplicate = dbContext.Labels.Any(l => l.Id != label.Id && l.Name == label.Name && l.Owner == label.Owner);
            
            if (duplicate)
                throw new InvalidOperationException($"Label with name {label.Name} already exists for owner {label.Owner?.Name??"ALL"}");
        }
    }
}