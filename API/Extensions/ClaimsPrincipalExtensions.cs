using System.Security.Claims;

namespace API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetUsernameClaim(this ClaimsPrincipal claims)
    {

        return claims.FindFirstValue(ClaimTypes.Name) ??
            throw new Exception("Cannot get username from token.");
    }

    public static int GetUserIdClaim(this ClaimsPrincipal claims)
    {

        return int.Parse(claims.FindFirstValue(ClaimTypes.NameIdentifier) ??
            throw new Exception("Cannot get user ID from token."));
    }
}
