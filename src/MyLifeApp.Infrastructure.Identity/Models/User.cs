using Microsoft.AspNetCore.Identity;

namespace MyLifeApp.Infrastructure.Identity.Models
{
    public class User : IdentityUser
    {
        public string? Name { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenValidityTime { get; set; }
    }
}
