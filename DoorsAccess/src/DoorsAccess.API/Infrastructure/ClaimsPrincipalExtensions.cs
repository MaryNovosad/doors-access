using System;
using System.Linq;
using System.Security.Claims;

namespace DoorsAccess.API.Infrastructure;

public static class ClaimsPrincipalExtensions
{
    public static long GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        var userIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            throw new ArgumentException($"User principal info is not complete: claim {ClaimTypes.NameIdentifier} is missing");
        }

        if (long.TryParse(userIdClaim.Value, out long userIdFromClaim))
        {
            return userIdFromClaim;
        }

        throw new ArgumentException($"User principal claim {ClaimTypes.NameIdentifier} value {userIdClaim.Value} is invalid");
    }
}