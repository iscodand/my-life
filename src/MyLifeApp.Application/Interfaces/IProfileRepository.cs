using MyLifeApp.Domain.Entities;

namespace MyLifeApp.Application.Interfaces
{
    public interface IProfileRepository : IGenericRepository<Profile>
    {
        public Task<Profile> GetProfileByUsername(string username);
        public Task<ICollection<ProfileFollower>> GetProfileFollowers(Profile profile);
        public Task<ICollection<ProfileFollower>> GetProfileFollowings(Profile profile);
        public Task<ProfileAnalytics> CreateProfileAnalytics(ProfileAnalytics profileAnalytics);
        public Task<ProfileFollower> CreateProfileFollower(ProfileFollower profileFollower);
        public Task RemoveProfileFollower(ProfileFollower profileFollower);
    }
}