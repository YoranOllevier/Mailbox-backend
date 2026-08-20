using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mailbox.Shared.Identity;

namespace Mailbox.Server.Endpoints.Identity.Roles;

/// <summary>
/// List all roles.
/// See https://fast-endpoints.com/ 
/// </summary>
/// <param name="roleManager"></param>
public class Index(RoleManager<IdentityRole> roleManager) : EndpointWithoutRequest<Result<List<KeyValuePair<string, string>>>>
{
    public override void Configure()
    {
        Get("/api/identity/roles");
        Roles(AppRoles.Admin);
    }

    public override async Task<Result<List<KeyValuePair<string, string>>>> ExecuteAsync(CancellationToken ctx)
    {
        var roles = await roleManager.Roles.Select(r => new KeyValuePair<string, string>(r.Id, r.Name!)).ToListAsync(ctx);
        return Result.Success(roles);
    }
}