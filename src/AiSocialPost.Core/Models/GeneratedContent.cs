namespace AiSocialPost.Core.Models
{
    public class GeneratedContent
    {
        public int Id { get; set; }
        public int TrendId { get; set; }
        public string Platform { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string Hashtags { get; set; } = string.Empty;
        public string Status { get; set; } = "Draft"; // Draft, Scheduled, Posted
        public DateTime? ScheduledFor { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? MediaUrl { get; set; }

        public TrendingTopic? Trend { get; set; }
    }
}
