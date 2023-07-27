using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyLifeApp.Infrastructure.Identity.DTOs.Request
{
    public class ResetPasswordRequest
    {
        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Token { get; set; }

        [Required(ErrorMessage = "Your New Password is required")]
        [StringLength(32, MinimumLength = 8, ErrorMessage = "Password must higher than 8 characters.")]
        public string? NewPassword { get; set; }

        [Compare("NewPassword")]
        [StringLength(32, MinimumLength = 8, ErrorMessage = "Password must higher than 8 characters.")]
        public string? ConfirmNewPassword { get; set; }
    }
}