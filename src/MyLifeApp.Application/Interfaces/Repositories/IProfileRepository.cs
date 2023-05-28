using MyLifeApp.Domain.Entities;

namespace MyLifeApp.Application.Interfaces.Repositories
{
    public interface IProfileRepository : IGenericRepository<Profile>
    {
        public Task<Profile> GetProfileByUsernameAsync(string username);
        public Task<ICollection<ProfileFollower>> GetProfileFollowersAsync(Profile profile);
        public Task<ICollection<ProfileFollower>> GetProfileFollowingsAsync(Profile profile);
        public Task<ProfileAnalytics> CreateProfileAnalyticsAsync(ProfileAnalytics profileAnalytics);
    }
}