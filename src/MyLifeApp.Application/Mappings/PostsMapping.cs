using MyLifeApp.Application.Dtos.Requests.Post;
using MyLifeApp.Application.Dtos.Responses.Post;
using MyLifeApp.Domain.Entities;
using Profile = AutoMapper.Profile;

namespace MyLifeApp.Application.Mappings
{
    public class PostsMapping : Profile
    {
        public PostsMapping()
        {
            CreateMap<CreatePostRequest, Post>();
            CreateMap<UpdatePostRequest, Post>();

            CreateMap<Post, GetPostsResponse>();
        }
    }
}
