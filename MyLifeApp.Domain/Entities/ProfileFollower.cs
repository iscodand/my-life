namespace MyLifeApp.Domain.Entities
{
    public class ProfileFollower
    {
        public Profile Profile { get; set; }
        public Guid ProfileId { get; set; }

        // Follower is also a Profile
        public Profile Follower { get; set; }
        public Guid FollowerId { get; set; }
    }
}
