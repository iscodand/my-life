using Identity.Infrastructure.Models;
using MyLifeApp.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyLifeApp.Domain.Entities
{
    public class Profile : BaseEntity
    {
        [ForeignKey("UserId")]
        public User? User { get; set; }
        public string? UserId { get; set; }

        public string? Bio { get; set; }
        public string? Location { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsActive { get; set; }
        public ICollection<ProfileFollower>? ProfileFollowers { get; set; }
        public ICollection<ProfileFollower>? ProfileFollowing { get; set; }
        public ICollection<Post>? Posts { get; set; }
        public ICollection<PostLike>? PostLikes { get; set; }
        public ICollection<PostComment>? PostComments { get; set; }

        public Profile()
        {
            CreatedAt = DateTime.Now;
            IsActive = true;
            IsPrivate = false;
        }
    }

    public class ProfileAnalytics : BaseEntity
    {
        public Profile? Profile { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
    }
}
