using AiSocialPost.Core.Models;
using AiSocialPost.Data;
using AiSocialPost.Platforms.Clients;
using AiSocialPost.AI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AiSocialPost.Core.Services
{
    public class CommentService : ICommentService
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly GeminiService _geminiService;

        public CommentService(AppDbContext dbContext, IConfiguration configuration, GeminiService geminiService)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _geminiService = geminiService;
        }

        public async Task MonitorCommentsAsync(string postId, string platform)
        {
            if (platform.ToLower() != "facebook")
            {
                // Only Facebook is implemented for now
                return;
            }

            var pageId = _configuration["Facebook:PageId"];
            var accessToken = _configuration["Facebook:PageAccessToken"];

            if (string.IsNullOrEmpty(pageId) || string.IsNullOrEmpty(accessToken))
            {
                return;
            }

            var facebookClient = new FacebookClient(pageId, accessToken);
            var fbComments = await facebookClient.GetCommentsAsync(postId);

            foreach (var fbComment in fbComments)
            {
                // Check if comment already exists in database
                var existingComment = await _dbContext.Comments
                    .FirstOrDefaultAsync(c => c.PostId == postId && c.UserId == fbComment.FromId);

                if (existingComment == null)
                {
                    // New comment - add to database
                    var comment = new Comment
                    {
                        PostId = postId,
                        Platform = "Facebook",
                        Text = fbComment.Message,
                        UserId = fbComment.FromId,
                        UserName = fbComment.From,
                        IsLead = DetectLead(fbComment.Message),
                        Replied = false,
                        CreatedAt = fbComment.CreatedTime
                    };

                    _dbContext.Comments.Add(comment);
                    await _dbContext.SaveChangesAsync();

                    // Auto-reply
                    await ReplyToCommentAsync(comment);

                    // Auto-like
                    await facebookClient.LikeCommentAsync(fbComment.Id);
                }
            }
        }

        public async Task ReplyToCommentAsync(Comment comment)
        {
            if (comment.Replied)
            {
                return; // Already replied
            }

            // Generate AI reply
            var replyText = await _geminiService.GenerateReplyAsync(comment.Text);

            // Post reply to platform
            if (comment.Platform.ToLower() == "facebook")
            {
                var pageId = _configuration["Facebook:PageId"];
                var accessToken = _configuration["Facebook:PageAccessToken"];

                if (!string.IsNullOrEmpty(pageId) && !string.IsNullOrEmpty(accessToken))
                {
                    var facebookClient = new FacebookClient(pageId, accessToken);

                    // Note: To reply to comment, we need the comment ID from Facebook
                    // For now, just save the reply text
                    comment.ReplyText = replyText;
                    comment.Replied = true;

                    _dbContext.Comments.Update(comment);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task<List<Comment>> GetLeadsAsync()
        {
            return await _dbContext.Comments
                .Where(c => c.IsLead)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        private bool DetectLead(string commentText)
        {
            var leadKeywords = new[] { "giá", "bao nhiêu", "mua", "order", "đặt hàng", "ship", "giao hàng", "có sẵn", "còn hàng" };
            var text = commentText.ToLower();

            return leadKeywords.Any(keyword => text.Contains(keyword));
        }
    }
}
