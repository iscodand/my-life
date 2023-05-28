using Microsoft.EntityFrameworkCore;
using MyLifeApp.Application.Interfaces.Repositories;
using MyLifeApp.Domain.Entities;
using MyLifeApp.Infrastructure.Data.Context;

namespace MyLifeApp.Infrastructure.Data.Repositories
{
    public class ProfileRepository : GenericRepository<Profile>, IProfileRepository
    {
        private readonly DbSet<Profile> _profiles;
        private readonly DbSet<ProfileFollower> _profileFollowers;
        private readonly DbSet<ProfileAnalytics> _analytics;

        public ProfileRepository(ApplicationDbContext context) : base(context)
        {
            _profiles = context.Set<Profile>();
            _profileFollowers = context.Set<ProfileFollower>();
            _analytics = context.Set<ProfileAnalytics>();
        }

        public async Task<Profile> GetProfileByUsernameAsync(string username)
        {
            return await _profiles.Where(p => p.User.NormalizedUserName == username.ToUpper().Trim())
                                  .Include(p => p.User)
                                  .FirstOrDefaultAsync();
        }

        public async Task<ICollection<ProfileFollower>> GetProfileFollowersAsync(Profile profile)
        {
            return await _profileFollowers.Where(pf => pf.Follower == profile)
                                    .Include(pf => pf.Follower.User)
                                    .ToListAsync();
        }

        public async Task<ICollection<ProfileFollower>> GetProfileFollowingsAsync(Profile profile)
        {
            return await _profileFollowers.Where(pf => pf.Profile == profile)
                                    .Include(pf => pf.Follower.User)
                                    .ToListAsync();
        }

        public async Task<ProfileAnalytics> GetProfileAnalyticsAsync(Profile profile)
        {
            return await _analytics.FirstOrDefaultAsync(p => p.Profile == profile);
        }

        public async Task<ProfileAnalytics> CreateProfileAnalyticsAsync(ProfileAnalytics profileAnalytics)
        {
            await _analytics.AddAsync(profileAnalytics);
            await base.SaveAsync();
            return profileAnalytics;
        }
    }
}
