using System.ComponentModel.DataAnnotations;

namespace MyLifeApp.Application.Dtos.Requests.Post
{
    public class CommentPostRequest
    {
        [StringLength(255, MinimumLength = 5)]
        public string? Comment { get; set; }
    }
}
