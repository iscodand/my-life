namespace MyLifeApp.Application.Dtos.Responses.Post
{
    public class GetAllPostsResponse : BaseResponse
    {
        public ICollection<GetPostsResponse>? Posts { get; set; }
    }
}
