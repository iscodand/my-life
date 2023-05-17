using System.ComponentModel.DataAnnotations;

namespace MyLifeApp.Application.Dtos.Requests.Post
{
    public class CreatePostRequest
    {
        [Required(ErrorMessage = "Post Title is required.")]
        [StringLength(128, MinimumLength = 5)]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Post Description is required.")]
        [StringLength(255, MinimumLength = 5)]
        public string? Description { get; set; }
        
        public bool IsPrivate { get; set; }
        public List<string>? Tags { get; set; }
    }
}
