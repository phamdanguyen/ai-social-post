using AiSocialPost.Core.Models;

namespace AiSocialPost.Core.Services
{
    public interface ITrendService
    {
        Task<List<TrendingTopic>> GetTrendsAsync();
        Task<TrendingTopic?> GetTrendByIdAsync(int id);
        Task RefreshTrendsAsync();
    }
}
