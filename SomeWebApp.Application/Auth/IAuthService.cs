using System.Collections.Generic;
using System.Security.Claims;

namespace SomeWebApp.Application.Auth
{
    public interface IAuthService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
