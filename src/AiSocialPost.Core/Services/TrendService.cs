using AiSocialPost.Core.Models;
using AiSocialPost.Data;
using Microsoft.EntityFrameworkCore;

namespace AiSocialPost.Core.Services
{
    public class TrendService : ITrendService
    {
        private readonly AppDbContext _dbContext;

        public TrendService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<TrendingTopic>> GetTrendsAsync()
        {
            return await _dbContext.TrendingTopics
                .OrderByDescending(t => t.DetectedAt)
                .Take(20)
                .ToListAsync();
        }

        public async Task<TrendingTopic?> GetTrendByIdAsync(int id)
        {
            return await _dbContext.TrendingTopics.FindAsync(id);
        }

        public async Task RefreshTrendsAsync()
        {
            // TODO: Implement Gemini-powered trend detection
            // For now, create sample data
            var sampleTrends = new List<TrendingTopic>
            {
                new TrendingTopic
                {
                    Platform = "Facebook",
                    Topic = "AI và Tương lai công nghệ",
                    Score = 85,
                    DetectedAt = DateTime.Now
                },
                new TrendingTopic
                {
                    Platform = "TikTok",
                    Topic = "Công thức nấu ăn 5 phút",
                    Score = 92,
                    DetectedAt = DateTime.Now
                },
                new TrendingTopic
                {
                    Platform = "YouTube",
                    Topic = "Hướng dẫn học tiếng Anh",
                    Score = 78,
                    DetectedAt = DateTime.Now
                }
            };

            _dbContext.TrendingTopics.AddRange(sampleTrends);
            await _dbContext.SaveChangesAsync();
        }
    }
}
