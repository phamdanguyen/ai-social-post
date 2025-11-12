using AiSocialPost.Core.Models;

namespace AiSocialPost.Core.Services
{
    public interface ICommentService
    {
        Task MonitorCommentsAsync(string postId, string platform);
        Task ReplyToCommentAsync(Comment comment);
        Task<List<Comment>> GetLeadsAsync();
    }
}
