using System.ComponentModel.DataAnnotations;

namespace MyLifeApp.Application.Dtos.Requests.Profile
{
    public class UpdateProfileRequest
    {
        [StringLength(90)]
        public string? Bio { get; set; }

        [StringLength(90)]
        public string? Location { get; set; }
        
        public DateTime BirthDate { get; set; }
        public bool IsPrivate { get; set; }
    }
}
