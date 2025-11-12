using AiSocialPost.Core.Models;
using AiSocialPost.Data;

namespace AiSocialPost.Core.Services
{
    public class PostingService : IPostingService
    {
        private readonly AppDbContext _dbContext;

        public PostingService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task PostNowAsync(GeneratedContent content, List<string> platforms)
        {
            // TODO: Implement actual posting to platforms
            // For now, just update status
            await Task.Delay(2000); // Simulate API call

            content.Status = "Posted";
            _dbContext.GeneratedContents.Update(content);
            await _dbContext.SaveChangesAsync();
        }

        public async Task SchedulePostAsync(GeneratedContent content, DateTime scheduledTime, List<string> platforms)
        {
            // TODO: Implement scheduling logic
            await Task.Delay(1000);

            content.Status = "Scheduled";
            content.ScheduledFor = scheduledTime;
            _dbContext.GeneratedContents.Update(content);
            await _dbContext.SaveChangesAsync();
        }
    }
}
