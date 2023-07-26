using System.ComponentModel.DataAnnotations;

namespace MyLifeApp.Infrastructure.Identity.DTOs.Request
{
    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "Access Token is required")]
        public string AccessToken { get; set; }

        [Required(ErrorMessage = "Refresh Token is required")]
        public string RefreshToken { get; set; }
    }
}
