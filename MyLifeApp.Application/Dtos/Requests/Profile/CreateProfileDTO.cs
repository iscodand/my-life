using Identity.Infrastructure.Models;

namespace MyLifeApp.Application.Dtos.Requests.Profile
{
    public class CreateProfileDTO
    {
        public User User { get; set; }
        public string Bio { get; set; }
        public string Location { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
