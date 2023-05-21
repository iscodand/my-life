using MyLifeApp.Application.Dtos.Responses.Profile;

namespace MyLifeApp.Application.Dtos.Responses.Post
{
    public class DetailPostResponse : BaseResponse
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public GetProfileResponse? Profile { get; set; }

        // public ICollection<GetTagsResponse> Tags { get; set; }
        // public ICollection<GetCommentsResponse> Comments { get; set; }
        // public ICollection<GetLikesResponse> Likes { get; set; }
    }
}
