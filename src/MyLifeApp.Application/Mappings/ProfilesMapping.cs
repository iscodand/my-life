using AutoMapper;
using MyLifeApp.Application.Dtos.Requests.Profile;

namespace MyLifeApp.Application.Mappings
{
    public class ProfilesMapping : Profile
    {
        public ProfilesMapping()
        {
            CreateMap<Domain.Entities.Profile, UpdateProfileRequest>();
            CreateMap<UpdateProfileRequest, Domain.Entities.Profile>();
        }
    }
}
