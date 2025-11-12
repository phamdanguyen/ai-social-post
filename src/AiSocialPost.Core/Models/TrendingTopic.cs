namespace AiSocialPost.Core.Models
{
    public class TrendingTopic
    {
        public int Id { get; set; }
        public string Platform { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public int Score { get; set; }           // 0-100
        public DateTime DetectedAt { get; set; }
        public string? AnalysisJson { get; set; } // AI analysis results (JSON)
    }
}
