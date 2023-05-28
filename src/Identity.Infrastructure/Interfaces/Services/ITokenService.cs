using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Identity.Infrastructure.Interfaces.Services
{
    public interface ITokenService
    {
        public JwtSecurityToken GenerateAccessToken(List<Claim> claims);
        public string GenerateRefreshToken();
        public ClaimsPrincipal GetPrincipalForExpiredToken(string token);
    }
}
