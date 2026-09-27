using Microsoft.AspNetCore.Identity;
using TaskManagerAPI.Models;

using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.DTOs;

namespace TaskManagerAPI.Services;

public class UserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }


    public async Task<UpdateRoleResult> UpdateRoleAsync(string userId, string newRole)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return UpdateRoleResult.UserNotFound;

        var roleExists = await _roleManager.RoleExistsAsync(newRole);

        if (!roleExists)
            return UpdateRoleResult.InvalidRole;

        var currentRoles = await _userManager.GetRolesAsync(user);

        if (currentRoles.Count == 1 && currentRoles[0] == newRole)
            return UpdateRoleResult.Success;

        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);

        if (!removeResult.Succeeded)
            return UpdateRoleResult.Failed;

        var addResult = await _userManager.AddToRoleAsync(user, newRole);

        if (!addResult.Succeeded)
        {
            await _userManager.AddToRolesAsync(user, currentRoles);
            return UpdateRoleResult.Failed;
        }

        return UpdateRoleResult.Success;
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await _userManager.Users.ToListAsync();

        var result = new List<UserDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            result.Add(new UserDto
            {
                Id = user.Id,
                Username = user.UserName ?? "",
                Role = roles.FirstOrDefault()
            });
        }

        return result;
    }
}