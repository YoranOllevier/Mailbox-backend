using Microsoft.Extensions.DependencyInjection;
using Mailbox.Persistence;
using Mailbox.Services.Accounts;
using Mailbox.Services.Mails;
using Mailbox.Shared.Accounts;
using Mailbox.Shared.Mails;

namespace Mailbox.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IMailService,MailService>();
        
        services.AddTransient<DbSeeder>();       
        
        return services;
    }
}