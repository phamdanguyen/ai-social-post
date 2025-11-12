using AiSocialPost.Core.Models;
using AiSocialPost.Data;
using Microsoft.EntityFrameworkCore;

namespace AiSocialPost.Core.Services
{
    public class ContentService : IContentService
    {
        private readonly AppDbContext _dbContext;

        public ContentService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GeneratedContent> GenerateContentAsync(TrendingTopic trend)
        {
            // TODO: Implement Gemini-powered content generation
            // For now, create sample content
            var content = new GeneratedContent
            {
                TrendId = trend.Id,
                Platform = trend.Platform,
                Text = $"[Sample Content] Bài viết về {trend.Topic}\n\nĐây là nội dung được tạo tự động bởi AI. Nội dung này sẽ được thay thế bởi Gemini API trong tương lai.",
                Hashtags = $"#AI #Trend #{trend.Platform}",
                Status = "Draft",
                CreatedAt = DateTime.Now
            };

            _dbContext.GeneratedContents.Add(content);
            await _dbContext.SaveChangesAsync();

            return content;
        }

        public async Task<List<GeneratedContent>> GetDraftsAsync()
        {
            return await _dbContext.GeneratedContents
                .Where(c => c.Status == "Draft")
                .Include(c => c.Trend)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task SaveContentAsync(GeneratedContent content)
        {
            _dbContext.GeneratedContents.Update(content);
            await _dbContext.SaveChangesAsync();
        }
    }
}
