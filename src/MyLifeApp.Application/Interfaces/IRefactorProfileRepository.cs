using MyLifeApp.Domain.Entities;

namespace MyLifeApp.Application.Interfaces
{
    public interface IRefactorProfileRepository : IGenericRepository<Profile>
    {
        public Task<Profile> GetProfileByUsername(string username);
        public Task<List<ProfileFollower>> GetProfileFollowers(Profile profile);
        public Task<List<ProfileFollower>> GetProfileFollowings(Profile profile);
        public Task<ProfileAnalytics> CreateProfileAnalytics(ProfileAnalytics profileAnalytics);
    }
}