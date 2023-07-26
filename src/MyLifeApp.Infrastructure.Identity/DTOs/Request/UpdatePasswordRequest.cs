using System.ComponentModel.DataAnnotations;

namespace MyLifeApp.Infrastructure.Identity.DTOs.Request
{
    public class UpdatePasswordRequest
    {
        [Required(ErrorMessage = "Your Old Password is required")]
        public string? OldPassword { get; set; }

        [Required(ErrorMessage = "Your New Password is required")]
        [StringLength(32, MinimumLength = 8, ErrorMessage = "Password must higher than 8 characters.")]
        public string? NewPassword { get; set; }

        [Compare("NewPassword")]
        [StringLength(32, MinimumLength = 8, ErrorMessage = "Password must higher than 8 characters.")]
        public string? ConfirmNewPassword { get; set; }
    }
}