using MyLifeApp.Application.Dtos.Responses.Profile;

namespace MyLifeApp.Application.Dtos.Responses.Post
{
    public class GetPostsResponse : BaseResponse
    {
        public string Title { get; set; }
        public GetProfileResponse Profile { get; set; }
        public int Likes { get; set; }
    }
}
