using AutoMapper;
using Identity.Infrastructure.Models;
using MyLifeApp.Application.Dtos.Requests.Profile;
using MyLifeApp.Application.Dtos.Responses.Profile;

namespace MyLifeApp.Application.Mappings
{
    public class ProfilesMapping : Profile
    {
        public ProfilesMapping()
        {
            CreateMap<Domain.Entities.Profile, UpdateProfileRequest>();
            CreateMap<UpdateProfileRequest, Domain.Entities.Profile>();

            CreateMap<GetProfileResponse, Domain.Entities.Profile>();
            CreateMap<Domain.Entities.Profile, GetProfileResponse>();

            CreateMap<GetUserResponse, User>();
            CreateMap<User, GetUserResponse>();
        }
    }
}
