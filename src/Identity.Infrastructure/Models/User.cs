using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Models
{
    public class User : IdentityUser
    {
        public string? Name { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenValidityTime { get; set; }
    }
}
