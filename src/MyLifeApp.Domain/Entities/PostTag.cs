using MyLifeApp.Domain.Common;

namespace MyLifeApp.Domain.Entities
{
    public class PostTag : BaseEntity
    {
        public Post? Post { get; set; }
        public int PostId { get; set; }
        public string? Name { get; set; }
    }
}
