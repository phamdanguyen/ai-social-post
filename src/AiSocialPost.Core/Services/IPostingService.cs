using AiSocialPost.Core.Models;

namespace AiSocialPost.Core.Services
{
    public interface IPostingService
    {
        Task PostNowAsync(GeneratedContent content, List<string> platforms);
        Task SchedulePostAsync(GeneratedContent content, DateTime scheduledTime, List<string> platforms);
    }
}
