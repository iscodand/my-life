using MyLifeApp.Domain.Common;

namespace MyLifeApp.Domain.Entities
{
    public class PostLike : BaseEntity
    {
        public Post Post { get; set; }
        public Guid PostId { get; set; }
        public Profile Profile { get; set; }
        public Guid ProfileId { get; set; }
    }
}
