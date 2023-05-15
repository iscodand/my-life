using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Identity.Infrastructure.Interfaces
{
    public interface ITokenRepository
    {
        public JwtSecurityToken GenerateAccessToken(List<Claim> claims);
        public string GenerateRefreshToken();
        public ClaimsPrincipal GetPrincipalForExpiredToken(string token);
    }
}
