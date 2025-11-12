using AiSocialPost.Core.Models;
using AiSocialPost.Data;
using AiSocialPost.AI;
using Microsoft.EntityFrameworkCore;

namespace AiSocialPost.Core.Services
{
    public class TrendService : ITrendService
    {
        private readonly AppDbContext _dbContext;
        private readonly GeminiService _geminiService;
        private readonly Random _random = new();

        public TrendService(AppDbContext dbContext, GeminiService geminiService)
        {
            _dbContext = dbContext;
            _geminiService = geminiService;
        }

        public async Task<List<TrendingTopic>> GetTrendsAsync()
        {
            var trends = await _dbContext.TrendingTopics
                .OrderByDescending(t => t.DetectedAt)
                .Take(20)
                .ToListAsync();

            // If no trends, refresh automatically
            if (trends.Count == 0)
            {
                await RefreshTrendsAsync();
                trends = await _dbContext.TrendingTopics
                    .OrderByDescending(t => t.DetectedAt)
                    .Take(20)
                    .ToListAsync();
            }

            return trends;
        }

        public async Task<TrendingTopic?> GetTrendByIdAsync(int id)
        {
            return await _dbContext.TrendingTopics.FindAsync(id);
        }

        public async Task RefreshTrendsAsync()
        {
            // Use Gemini to detect real trends
            var trendTopics = await _geminiService.DetectTrendsAsync("Vietnam");

            var platforms = new[] { "Facebook", "TikTok", "YouTube", "Twitter", "Zalo" };

            var newTrends = new List<TrendingTopic>();

            foreach (var topic in trendTopics)
            {
                // Assign random platform and score
                var platform = platforms[_random.Next(platforms.Length)];
                var score = _random.Next(60, 100);

                var trend = new TrendingTopic
                {
                    Platform = platform,
                    Topic = topic,
                    Score = score,
                    DetectedAt = DateTime.Now
                };

                newTrends.Add(trend);
            }

            if (newTrends.Count > 0)
            {
                _dbContext.TrendingTopics.AddRange(newTrends);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
