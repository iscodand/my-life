using AutoMapper;
using Identity.Infrastructure.Models;
using MyLifeApp.Application.Dtos.Requests.Post;
using MyLifeApp.Application.Dtos.Requests.Profile;
using MyLifeApp.Application.Dtos.Responses.Post;
using MyLifeApp.Application.Dtos.Responses.Profile;
using MyLifeApp.Domain.Entities;
using Profile = MyLifeApp.Domain.Entities.Profile;

namespace MyLifeApp.Application.Mappings
{
    public class ProfilesMapping : AutoMapper.Profile
    {
        public ProfilesMapping()
        {
            CreateMap<Profile, UpdateProfileRequest>();
            CreateMap<UpdateProfileRequest, Profile>();

            CreateMap<GetProfileResponse, Profile>();
            CreateMap<Profile, GetProfileResponse>();

            CreateMap<ProfileFollower, GetProfileResponse>();

            CreateMap<CommentPostRequest, PostComment>();
            CreateMap<PostComment, CommentPostRequest>();
            
            CreateMap<PostComment, GetPostCommentsDTO>();
        }
    }
}
