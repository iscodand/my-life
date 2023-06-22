using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Identity.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using MyLifeApp.Domain.Entities;

namespace MyLifeApp.Infrastructure.Data.Context
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Post
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostAnalytics> PostAnalytics { get; set; }
        public DbSet<PostLike> PostLikes { get; set; }
        public DbSet<PostComment> PostComments { get; set; }
        public DbSet<PostTag> PostTags { get; set; }

        // Profile
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<ProfileAnalytics> ProfileAnalytics { get; set; }
        public DbSet<ProfileFollower> ProfileFollowers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Many-to-many: PostLike Entity
            builder.Entity<PostLike>()
                .HasKey(pl => new { pl.ProfileId, pl.PostId });
            builder.Entity<PostLike>()
                .HasOne(p => p.Post)
                .WithMany(p => p.PostLikes)
                .HasForeignKey(p => p.PostId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<PostLike>()
                .HasOne(p => p.Profile)
                .WithMany(p => p.PostLikes)
                .HasForeignKey(p => p.ProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            // Many-to-many: PostComment Entity
            builder.Entity<PostComment>()
                .HasKey(pc => new { pc.PostId, pc.ProfileId });
            builder.Entity<PostComment>()
                .HasOne(p => p.Post)
                .WithMany(p => p.PostComments)
                .HasForeignKey(p => p.PostId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<PostComment>()
                .HasIndex(p => p.PostId)
                .IsUnique(false);
            builder.Entity<PostComment>()
                .HasOne(p => p.Profile)
                .WithMany(p => p.PostComments)
                .HasForeignKey(p => p.ProfileId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<PostComment>()
                .HasIndex(p => p.ProfileId)
                .IsUnique(false);

            // Many-to-many: PostTags Entity
            builder.Entity<PostTag>()
                .HasKey(pt => new { pt.PostId });
            builder.Entity<PostTag>()
                .HasOne(p => p.Post)
                .WithMany(p => p.PostTags)
                .HasForeignKey(p => p.PostId);

            // Many-to-many: PostTags Entity
            builder.Entity<ProfileFollower>()
                .HasKey(pf => new { pf.ProfileId, pf.FollowerId });
            builder.Entity<ProfileFollower>()
                .HasOne(p => p.Profile)
                .WithMany(p => p.ProfileFollowers)
                .HasForeignKey(p => p.ProfileId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<ProfileFollower>()
                .HasOne(p => p.Follower)
                .WithMany(p => p.ProfileFollowing)
                .HasForeignKey(p => p.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(builder);
            builder.Entity<User>(entity =>
            {
                entity.ToTable(name: "Users");
            });

            builder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable(name: "Roles");
            });
            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRoles");
            });

            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims");
            });

            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins");
            });

            builder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("RoleClaims");

            });

            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens");
            });
        }
    }
}
