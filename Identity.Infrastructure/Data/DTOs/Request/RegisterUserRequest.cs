using System.ComponentModel.DataAnnotations;

namespace Identity.Infrastructure.Data.DTOs.Request
{
    public class RegisterUserRequest
    {
        [Required(ErrorMessage = "Your Name is required")]
        [StringLength(128, MinimumLength = 5)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Your Username is required")]
        [StringLength(128, MinimumLength = 3)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Your Email is required")]
        [StringLength(128, MinimumLength = 5)]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Your Password is required")]
        [StringLength(64, MinimumLength = 8)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Access Token is required")]
        [Compare("Password")]
        public string PasswordConfirm { get; set; }
    }
}
