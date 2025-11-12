namespace AiSocialPost.Core.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string PostId { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public bool IsLead { get; set; }
        public bool Replied { get; set; }
        public string? ReplyText { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
