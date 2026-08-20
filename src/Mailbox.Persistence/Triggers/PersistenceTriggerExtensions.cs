using Microsoft.EntityFrameworkCore;

namespace Mailbox.Persistence.Triggers;

public static class PersistenceTriggerExtensions
{
    public static void AddApplicationTriggers(this DbContextOptionsBuilder o)
    {
        o.UseTriggers(options =>
            {
                options.AddTrigger<EntityBeforeSaveTrigger>();
                options.AddTrigger<LabelBeforeSaveTrigger>();
            }
        );
    }
}