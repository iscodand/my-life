namespace MyLifeApp.Application.Dtos.Requests.Post
{
    public class CreatePostRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsPrivate { get; set; }
        public List<string> Tags { get; set; }
    }
}
