using Identity.Infrastructure.Models;

namespace MyLifeApp.Application.Dtos.Responses.Profile
{
    public class GetProfileResponse
    {
        public Guid Id { get; set; }
        public GetUserResponse User { get; set; }
        public bool IsPrivate { get; set; }
    }
}
