using Microsoft.EntityFrameworkCore;
using AiSocialPost.Core.Models;

namespace AiSocialPost.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<TrendingTopic> TrendingTopics { get; set; } = null!;
        public DbSet<GeneratedContent> GeneratedContents { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // TrendingTopic configuration
            modelBuilder.Entity<TrendingTopic>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Platform).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Topic).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Score).IsRequired();
                entity.Property(e => e.DetectedAt).IsRequired();
            });

            // GeneratedContent configuration
            modelBuilder.Entity<GeneratedContent>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Platform).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Text).IsRequired();
                entity.Property(e => e.Hashtags).HasMaxLength(500);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);

                entity.HasOne(e => e.Trend)
                    .WithMany()
                    .HasForeignKey(e => e.TrendId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Comment configuration
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PostId).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Platform).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Text).IsRequired();
                entity.Property(e => e.UserId).IsRequired().HasMaxLength(100);
                entity.Property(e => e.UserName).HasMaxLength(200);
            });
        }
    }
}
