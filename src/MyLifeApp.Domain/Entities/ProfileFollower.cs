namespace MyLifeApp.Domain.Entities
{
    public class ProfileFollower
    {
        public Profile? Profile { get; set; }
        public int? ProfileId { get; set; }

        // Follower is also a Profile
        public Profile? Follower { get; set; }
        public int? FollowerId { get; set; }
    }
}
