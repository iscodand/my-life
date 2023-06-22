namespace MyLifeApp.Domain.Entities
{
    public class ProfileFollower
    {
        public Profile Profile { get; set; }
        public string ProfileId { get; set; }

        // Follower is also a Profile
        public Profile Follower { get; set; }
        public string FollowerId { get; set; }
    }
}
