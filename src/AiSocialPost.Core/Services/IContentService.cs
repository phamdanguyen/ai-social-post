using AiSocialPost.Core.Models;

namespace AiSocialPost.Core.Services
{
    public interface IContentService
    {
        Task<GeneratedContent> GenerateContentAsync(TrendingTopic trend);
        Task<List<GeneratedContent>> GetDraftsAsync();
        Task SaveContentAsync(GeneratedContent content);
    }
}
