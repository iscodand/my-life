using MyLifeApp.Application.Dtos.Responses.Profile;

namespace MyLifeApp.Application.Dtos.Responses.Post
{
    public class GetPostsResponse
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public GetProfileResponse? Profile { get; set; }
        public int Likes { get; set; }
        public int Comments { get; set; }
    }
}
