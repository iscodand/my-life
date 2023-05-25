using MyLifeApp.Domain.Entities;
using MyLifeApp.Infrastructure.Data.Context;
using MyLifeApp.Application.Interfaces;
using MyLifeApp.Application.Dtos.Responses;
using MyLifeApp.Application.Dtos.Requests.Post;

namespace MyLifeApp.Infrastructure.Data.Repositories
{
    public class RefactorPostRepository : GenericRepository<Post>, IRefactorPostRepository
    {
        public RefactorPostRepository(ApplicationDbContext context) : base(context)
        { }

        public Task<BaseResponse> CommentPost(Guid postId, CommentPostRequest postRequest)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponse> LikePost(Guid postId)
        {
            throw new NotImplementedException();
        }
    }
}