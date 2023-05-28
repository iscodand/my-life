using Microsoft.EntityFrameworkCore;
using MyLifeApp.Application.Interfaces;
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

        public async Task<Profile> GetProfileByUsername(string username)
        {
            return await _profiles.Where(p => p.User.NormalizedUserName == username.ToUpper().Trim())
                                  .Include(p => p.User)
                                  .FirstOrDefaultAsync();
        }

        public async Task<ICollection<ProfileFollower>> GetProfileFollowers(Profile profile)
        {
            return await _profileFollowers.Where(pf => pf.Follower == profile)
                                    .Include(pf => pf.Follower.User)
                                    .ToListAsync();
        }

        public async Task<ICollection<ProfileFollower>> GetProfileFollowings(Profile profile)
        {
            return await _profileFollowers.Where(pf => pf.Profile == profile)
                                    .Include(pf => pf.Follower.User)
                                    .ToListAsync();
        }

        public async Task<ProfileAnalytics> GetProfileAnalytics(Profile profile)
        {
            return await _analytics.FirstOrDefaultAsync(p => p.Profile == profile);
        }

        public async Task<ProfileAnalytics> CreateProfileAnalytics(ProfileAnalytics profileAnalytics)
        {
            await _analytics.AddAsync(profileAnalytics);
            await base.Save();
            return profileAnalytics;
        }

        public async Task<ProfileFollower> CreateProfileFollower(ProfileFollower profileFollower)
        {
            await _profileFollowers.AddAsync(profileFollower);
            await base.Save();
            return profileFollower;
        }

        public async Task RemoveProfileFollower(ProfileFollower profileFollower)
        {
            _profileFollowers.Remove(profileFollower);
            await base.Save();
        }
    }
}
