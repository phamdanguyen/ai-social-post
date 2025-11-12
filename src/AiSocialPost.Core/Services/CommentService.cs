using AiSocialPost.Core.Models;
using AiSocialPost.Data;
using Microsoft.EntityFrameworkCore;

namespace AiSocialPost.Core.Services
{
    public class CommentService : ICommentService
    {
        private readonly AppDbContext _dbContext;

        public CommentService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task MonitorCommentsAsync(string postId, string platform)
        {
            // TODO: Implement comment monitoring
            await Task.Delay(1000);
        }

        public async Task ReplyToCommentAsync(Comment comment)
        {
            // TODO: Implement AI reply generation and posting
            await Task.Delay(1000);

            comment.Replied = true;
            comment.ReplyText = "[Sample Reply] Cảm ơn bạn đã comment!";
            _dbContext.Comments.Update(comment);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Comment>> GetLeadsAsync()
        {
            return await _dbContext.Comments
                .Where(c => c.IsLead)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }
    }
}
