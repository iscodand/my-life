using Identity.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Identity.Infrastructure.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly IConfiguration _configuration;

        public TokenRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SymmetricSecurityKey AuthSigningKey()
        {
            SymmetricSecurityKey authSigningKey = new(Encoding.UTF8.GetBytes(_configuration["JWTSettings:Key"]));
            return authSigningKey;
        }


        public JwtSecurityToken GenerateAccessToken(List<Claim> claims)
        {
            _ = int.TryParse(_configuration["JWTSettings:AccessTokenValidityInMinutes"],
                out int accessTokenValidityTime);

            JwtSecurityToken jwtSecurityToken = new(
                claims: claims,
                expires: DateTime.Now.AddMinutes(accessTokenValidityTime),
                signingCredentials: new SigningCredentials(AuthSigningKey(), SecurityAlgorithms.HmacSha256)
            );

            return jwtSecurityToken;
        }

        public string GenerateRefreshToken()
        {
            byte[] randomNumber = new byte[32];
            using RandomNumberGenerator generator = RandomNumberGenerator.Create();
            generator.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);

        }

        public ClaimsPrincipal GetPrincipalForExpiredToken(string token)
        {
            TokenValidationParameters tokenValidationParameters = new()
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = AuthSigningKey(),
            };

            JwtSecurityTokenHandler tokenHandler = new();

            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationParameters,
                out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
    }
}
