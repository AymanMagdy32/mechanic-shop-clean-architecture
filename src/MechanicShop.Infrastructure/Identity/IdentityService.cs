
using Azure.Core;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace MechanicShop.Infrastructure.Identity;

public class IdentityService(UserManager<AppUser> userManager, IUserClaimsPrincipalFactory<AppUser> userClaimsPrincipalFactory , IAuthorizationService authorizationService) : IIdentityService
{
    public async Task<Result<AppUserDto>> AuthenticateAsync(string email, string password)
    {
        var user = await userManager.FindByEmailAsync(email); 
        if(user == null )
        return Error.NotFound("User_Not_Found", $"User with email {email} not found");
        
        if(user.EmailConfirmed == false)
            return Error.Forbidden("Email_Not_Confirmed", $"Email for user with email {email} is not confirmed");

      if(await userManager.CheckPasswordAsync(user, password) == false)
            return Error.Unauthorized("Invalid_Credentials", $"Invalid credentials for user with email {email}");

        var roles = await userManager.GetRolesAsync(user);
        var claims = await userManager.GetClaimsAsync(user);

        return new AppUserDto(user.Id, user.Email!, roles, claims);
    }

    public async Task<bool> AuthorizeAsync(string userId, string? policyName)
    {
      var user = await userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return false;
        }

        var principal = await userClaimsPrincipalFactory.CreateAsync(user);

        var result = await authorizationService.AuthorizeAsync(principal, policyName!);

        return result.Succeeded;
       
    }

    public async Task<Result<AppUserDto>> GetUserByIdAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId) ?? throw new InvalidOperationException(nameof(userId));

        var roles = await userManager.GetRolesAsync(user);

        var claims = await userManager.GetClaimsAsync(user);

        return new AppUserDto(user.Id, user.Email!, roles, claims);
    }

    public async Task<string?> GetUserNameAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        return user?.UserName;
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        return await userManager.IsInRoleAsync(user, role);
    }
}