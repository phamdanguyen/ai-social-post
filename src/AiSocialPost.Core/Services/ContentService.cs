using AiSocialPost.Core.Models;
using AiSocialPost.Data;
using AiSocialPost.AI;
using Microsoft.EntityFrameworkCore;

namespace AiSocialPost.Core.Services
{
    public class ContentService : IContentService
    {
        private readonly AppDbContext _dbContext;
        private readonly GeminiService _geminiService;

        public ContentService(AppDbContext dbContext, GeminiService geminiService)
        {
            _dbContext = dbContext;
            _geminiService = geminiService;
        }

        public async Task<GeneratedContent> GenerateContentAsync(TrendingTopic trend)
        {
            // Use Gemini to generate real content
            var postContent = await _geminiService.GeneratePostFromTrendAsync(trend.Topic, trend.Platform);
            var hashtags = await _geminiService.GenerateHashtagsAsync(postContent);

            var content = new GeneratedContent
            {
                TrendId = trend.Id,
                Platform = trend.Platform,
                Text = postContent,
                Hashtags = hashtags,
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
