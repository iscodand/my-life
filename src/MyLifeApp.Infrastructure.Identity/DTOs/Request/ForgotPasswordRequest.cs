using System.ComponentModel.DataAnnotations;

namespace MyLifeApp.Infrastructure.Identity.DTOs.Request
{
    public class ForgetPasswordRequest
    {
        [EmailAddress]
        public string? Email { get; set; }
    }
}
