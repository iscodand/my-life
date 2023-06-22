using MyLifeApp.Domain.Common;

namespace MyLifeApp.Domain.Entities
{
    public class Post : BaseEntity
    {
        public Profile? Profile { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool IsPrivate { get; set; }
        public ICollection<PostTag>? PostTags { get; set; }
        public ICollection<PostLike>? PostLikes { get; set; }
        public ICollection<PostComment>? PostComments { get; set; }
    }

    public class PostAnalytics : BaseEntity
    {
        public Post? Post { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
    }
}
