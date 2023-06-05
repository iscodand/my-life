using MyLifeApp.Domain.Entities;

namespace MyLifeApp.Application.Interfaces.Services
{
    public interface IAuthenticatedProfileService
    {
        public Task<Profile> GetAuthenticatedProfile();
    }
}