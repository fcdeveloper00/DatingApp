using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AdminController : BaseApiController
{
    readonly UserManager<AppUser> _userManager;
    public AdminController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    [Authorize(policy: "RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult> GetUsersWithRoles()
    {
        var users = await _userManager.Users.OrderBy(u => u.UserName)
             .Select(u => new
             {
                 u.Id,
                 u.UserName,
                 Roles = u.UserRoles.Select(ur => ur.Role.Name).ToArray()
             }).ToListAsync();

        return Ok(users);
        //return Ok("Hi, Admin... ");
    }

    [HttpPost("edit-roles/{username}")]
    [Authorize(policy: "RequireAdminRole")]
    public async Task<ActionResult> EditRoles(string username, string roles)
    {
        if (string.IsNullOrEmpty(roles)) return BadRequest("You must select at least one role.");

        string[] selectedRoles = roles.Split(',').ToArray();

        var user = await _userManager.FindByNameAsync(username);

        if (user == null) return BadRequest("Cannot find user");

        var currentRoles = await _userManager.GetRolesAsync(user);

        var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(currentRoles));

        if (!result.Succeeded) return BadRequest("Failed to add to roles");

        result = await _userManager.RemoveFromRolesAsync(user, currentRoles.Except(selectedRoles));

        if (!result.Succeeded) return BadRequest("Cannot update user roles");

        return Ok(await  _userManager.GetRolesAsync(user));

    }


    [Authorize(policy: "ModeratePhotoRole")]
    [HttpGet("photos-to-moderate")]
    public ActionResult GetPhotosForModeration()
    {
        return Ok("Moderating photos...");
    }
}
