using API.Entities;
using API.Interfaces;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Services;

public class TokenService : ITokenService
{
    readonly IConfiguration _configuration;
    readonly UserManager<AppUser> _userManager;
    public TokenService(IConfiguration configuration, UserManager<AppUser> userManager)
    {
        _configuration = configuration;
        _userManager = userManager;
    }
    public async Task<string> CreateTokenAsync(AppUser user)
    {
        var tokenKey = _configuration["TokenKey"] ?? throw new NullReferenceException("Cannot access TokenKey from the app.settings.json file.");

        if (tokenKey.Length < 64)
            throw new Exception("The TokenKey needs to be longer");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

        if (user.UserName == null) throw new Exception("No username for user.");

        var claims = new List<Claim>
        {
          new(ClaimTypes.NameIdentifier,user.Id.ToString()),
          new(ClaimTypes.Name,user.UserName)
        };

        IList<string> userRoles = await _userManager.GetRolesAsync(user);

        claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));


        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
