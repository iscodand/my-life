using MyLifeApp.Application.Dtos.Responses.Profile;

namespace MyLifeApp.Application.Dtos.Responses.Post
{
    public class DetailPostResponse : BaseResponse
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public GetProfileResponse? Profile { get; set; }
        public int Likes { get; set; }
        public ICollection<GetPostCommentsDTO>? Comments { get; set; }
    }

    public class GetPostCommentsDTO
    {
        public string? ProfileUserUsername { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
