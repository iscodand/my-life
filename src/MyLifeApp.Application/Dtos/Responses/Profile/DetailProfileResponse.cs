using MyLifeApp.Application.Dtos.Responses.Post;

namespace MyLifeApp.Application.Dtos.Responses.Profile
{
    public class DetailProfileResponse : BaseResponse
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Username { get; set; }
        public string? Bio { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsPrivate { get; set; }
        public ICollection<GetPostsResponse>? Posts { get; set; }
    }
}
