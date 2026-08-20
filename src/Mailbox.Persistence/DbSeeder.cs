using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mailbox.Domain.Accounts;
using Mailbox.Domain.Labels;
using Mailbox.Domain.Mails;

namespace Mailbox.Persistence;
/// <summary>
/// Seeds the database
/// </summary>
/// <param name="dbContext"></param>
/// <param name="roleManager"></param>
/// <param name="userManager"></param>
public class DbSeeder(ApplicationDbContext dbContext, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
{
    const string PasswordDefault = "A1b2C3!";
    
    public async Task SeedAsync()
    {
        await RolesAsync();
        await UsersAsync();
        await AccountAsync();
        await MailsAsync();
    }

    private async Task RolesAsync()
    {
        if (dbContext.Roles.Any())
            return;

        await roleManager.CreateAsync(new IdentityRole("Admin"));
        await roleManager.CreateAsync(new IdentityRole("User"));
        
    }

    private async Task  UsersAsync()
    {
        if (dbContext.Users.Any())
            return;
        
        await dbContext.Roles.ToListAsync();

        var user = new IdentityUser
        {
            UserName = "user@example.be",
            Email = "user@example.be",
            EmailConfirmed = true,
        };
        await userManager.CreateAsync(user, PasswordDefault);
        await userManager.AddToRoleAsync(user, "User");
        
        var admin = new IdentityUser
        {
            UserName = "admin@example.be",
            Email = "admin@example.be",
            EmailConfirmed = true,
        };
        await userManager.CreateAsync(admin, PasswordDefault);
        await userManager.AddToRoleAsync(admin, "Admin");

        await dbContext.SaveChangesAsync();
    }

    private async Task AccountAsync()
    {
        if(dbContext.Accounts.Any())
            return;
        
        var users = await dbContext.Users.ToListAsync();
        users.ForEach(u => dbContext.Accounts.Add(new Account(u.UserName!, u.Email!, u.Id)));
        
        await dbContext.SaveChangesAsync();
    }
    
    private async Task MailsAsync()
    {
        if (dbContext.Mails.Any())
            return;

        var accounts = await dbContext.Accounts.ToListAsync();
        var user = accounts.First(a => a.Email.Address == "user@example.be");
        
        #region Labels

        var snoozed = new Label("snoozed");
        await dbContext.Labels.AddAsync(snoozed);
        
        var important = new Label("important");
        await dbContext.Labels.AddAsync(important);
        
        var scheduled = new Label("scheduled");
        await dbContext.Labels.AddAsync(scheduled);
        
        var spam = new Label("spam");
        await dbContext.Labels.AddAsync(spam);
        
        var trash = new Label("trash");
        await dbContext.Labels.AddAsync(trash);
        await dbContext.SaveChangesAsync();
        
        var trash2 = new Label("trash", user);
        await dbContext.Labels.AddAsync(trash2);
        
        var newsletter = new Label("newsletter", user);
        await dbContext.Labels.AddAsync(newsletter);

        await dbContext.SaveChangesAsync();
        #endregion

        var m1 = new Mail(user,
            new HashSet<Account> { user },
            "Welcome to Mailbox",
            "Welcome to your new mailbox!",
            DateTime.UtcNow.AddDays(-2),
            true,
            true,
            false,
            new HashSet<Label> { newsletter });
        await dbContext.Mails.AddAsync(m1);
        
        var m2 = new Mail(user,
            new HashSet<Account> { user },
            "Welcome to Mailbox",
            "Welcome to your new mailbox!",
            DateTime.UtcNow.AddDays(-32),
            false,
            true,
            false,
            new HashSet<Label> { important });
        await dbContext.Mails.AddAsync(m2);

        await dbContext.SaveChangesAsync();
    }
}