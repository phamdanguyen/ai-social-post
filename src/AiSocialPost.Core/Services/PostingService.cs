using AiSocialPost.Core.Models;
using AiSocialPost.Data;
using AiSocialPost.Platforms.Clients;
using Microsoft.Extensions.Configuration;

namespace AiSocialPost.Core.Services
{
    public class PostingService : IPostingService
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public PostingService(AppDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task PostNowAsync(GeneratedContent content, List<string> platforms)
        {
            var results = new List<string>();

            foreach (var platform in platforms)
            {
                try
                {
                    switch (platform.ToLower())
                    {
                        case "facebook":
                            await PostToFacebookAsync(content);
                            results.Add($"✅ Facebook: Success");
                            break;

                        case "tiktok":
                        case "youtube":
                        case "zalo":
                        case "twitter":
                            results.Add($"⏳ {platform}: Not implemented yet");
                            break;

                        default:
                            results.Add($"❌ {platform}: Unknown platform");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    results.Add($"❌ {platform}: {ex.Message}");
                }
            }

            content.Status = "Posted";
            _dbContext.GeneratedContents.Update(content);
            await _dbContext.SaveChangesAsync();
        }

        private async Task PostToFacebookAsync(GeneratedContent content)
        {
            var pageId = _configuration["Facebook:PageId"];
            var accessToken = _configuration["Facebook:PageAccessToken"];

            if (string.IsNullOrEmpty(pageId) || string.IsNullOrEmpty(accessToken))
            {
                throw new Exception("Facebook Page ID hoặc Access Token chưa được cấu hình. Vui lòng vào Settings.");
            }

            var facebookClient = new FacebookClient(pageId, accessToken);

            var message = $"{content.Text}\n\n{content.Hashtags}";
            var postId = await facebookClient.PostAsync(message);

            if (string.IsNullOrEmpty(postId))
            {
                throw new Exception("Post failed - No post ID returned");
            }

            // Save post ID for later comment monitoring
            content.MediaUrl = $"fb://{postId}"; // Store postId in MediaUrl field
        }

        public async Task SchedulePostAsync(GeneratedContent content, DateTime scheduledTime, List<string> platforms)
        {
            // TODO: Implement scheduling logic with background job
            await Task.Delay(1000);

            content.Status = "Scheduled";
            content.ScheduledFor = scheduledTime;
            _dbContext.GeneratedContents.Update(content);
            await _dbContext.SaveChangesAsync();
        }
    }
}
