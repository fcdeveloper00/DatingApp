using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace API.Data;

public class Seed()
{
    public async static Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        // Check the database for presence of any data.
        if (await userManager.Users.AnyAsync())
        {
            return;
        }

        var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        var users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);

        if (users is null) return;

        var roles = new List<AppRole>()
        {
            new(){Name = "Admin"},
            new(){Name = "Moderator"},
            new(){Name = "Member"}
        };

        foreach (var role in roles)
        {
            await roleManager.CreateAsync(role);
        }

        AppUser admin = new()
        {
            UserName = "admin",
            KnownAs = "Admin",
            City = "",
            Country = "",
            Gender = "",
        };

        await userManager.CreateAsync(admin, "Pa$$w0rd");
        await userManager.AddToRolesAsync(admin, ["Admin", "Moderator"]);

        foreach (var user in users)
        {
            //user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
            //user.PasswordSalt = hmac.Key;
            user.UserName = user.UserName!.ToLower();
            await userManager.CreateAsync(user, "Pa$$w0rd");
            await userManager.AddToRoleAsync(user, "Member");
        }
    }

}
