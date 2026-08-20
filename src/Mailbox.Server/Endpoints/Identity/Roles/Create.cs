using Microsoft.AspNetCore.Identity;
using Mailbox.Shared.Identity;
using Mailbox.Shared.Identity.Roles;

namespace Mailbox.Server.Endpoints.Identity.Roles;

/// <summary>
/// Create a new role.
/// See https://fast-endpoints.com/
/// </summary>
/// <param name="roleManager"></param>
public class Create(RoleManager<IdentityRole> roleManager) : Endpoint<RoleRequest.Create, Result<string>>
{
    public override void Configure()
    {
        Post("/api/identity/roles");
        Roles(AppRoles.Admin);
    }

    public override async Task<Result<string>> ExecuteAsync(RoleRequest.Create req, CancellationToken ctx)
    {
        if(await roleManager.RoleExistsAsync(req.Name))
            return Result.Conflict($"Role with name '{req.Name}' already exists.");
        
        IdentityRole role = new()
        {
            Name = req.Name,
            NormalizedName = req.Name.ToUpper()
        };

        var result = await roleManager.CreateAsync(role);
        
        if(!result.Succeeded)
            return Result.Error(result.Errors.First().Description);
        
        return Result.Created(role.Id);
    }
}