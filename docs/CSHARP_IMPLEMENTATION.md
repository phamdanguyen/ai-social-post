# Hướng Dẫn Implementation C# WinForms

## Tổng Quan Kiến Trúc

### Layered Architecture

```
┌─────────────────────────────────────────────┐
│         Presentation Layer (WinForms)        │
│  - Forms, Controls, User Interactions       │
└──────────────────┬──────────────────────────┘
                   │
┌──────────────────▼──────────────────────────┐
│          Business Logic Layer                │
│  - Services, Workflows, Business Rules      │
└──────────────────┬──────────────────────────┘
                   │
┌──────────────────▼──────────────────────────┐
│         Integration Layer                    │
│  - Social Media APIs, AI APIs               │
└──────────────────┬──────────────────────────┘
                   │
┌──────────────────▼──────────────────────────┐
│          Data Access Layer                   │
│  - Repository Pattern, Entity Framework     │
└──────────────────┬──────────────────────────┘
                   │
┌──────────────────▼──────────────────────────┐
│             Database                         │
│  - SQL Server / SQLite                      │
└─────────────────────────────────────────────┘
```

---

## 1. DATA MODELS

### 1.1 TrendingTopic.cs

```csharp
using System;
using System.Collections.Generic;

namespace AiSocialPost.Core.Models
{
    public class TrendingTopic
    {
        public Guid Id { get; set; }
        public string Platform { get; set; } // TikTok, YouTube, Google, etc.
        public string Topic { get; set; }
        public List<string> Hashtags { get; set; }
        public int EngagementScore { get; set; }
        public DateTime FirstSeen { get; set; }
        public DateTime? PeakTime { get; set; }
        public TrendStatus Status { get; set; }
        public Dictionary<string, object> Metadata { get; set; }

        // Navigation properties
        public virtual ICollection<ContentSource> ContentSources { get; set; }
    }

    public enum TrendStatus
    {
        Rising,
        Peak,
        Declining,
        Archived
    }
}
```

### 1.2 ContentSource.cs

```csharp
using System;

namespace AiSocialPost.Core.Models
{
    public class ContentSource
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public string Platform { get; set; }
        public string ContentType { get; set; } // video, image, text
        public string OriginalContent { get; set; } // JSON serialized
        public string Transcript { get; set; }
        public string LocalFilePath { get; set; }
        public DateTime DownloadDate { get; set; }

        // Foreign keys
        public Guid TrendingTopicId { get; set; }
        public virtual TrendingTopic TrendingTopic { get; set; }

        // Navigation
        public virtual AnalyzedContent AnalyzedContent { get; set; }
    }
}
```

### 1.3 AnalyzedContent.cs

```csharp
using System;
using System.Collections.Generic;

namespace AiSocialPost.Core.Models
{
    public class AnalyzedContent
    {
        public Guid Id { get; set; }
        public Guid SourceId { get; set; }
        public virtual ContentSource Source { get; set; }

        public List<string> KeyTopics { get; set; }
        public string Tone { get; set; }
        public string TargetAudience { get; set; }
        public List<string> ViralElements { get; set; }
        public Dictionary<string, object> Analysis { get; set; }
        public DateTime AnalyzedAt { get; set; }

        // Navigation
        public virtual ICollection<GeneratedContent> GeneratedContents { get; set; }
    }
}
```

### 1.4 GeneratedContent.cs

```csharp
using System;
using System.Collections.Generic;

namespace AiSocialPost.Core.Models
{
    public class GeneratedContent
    {
        public Guid Id { get; set; }
        public Guid AnalyzedContentId { get; set; }
        public virtual AnalyzedContent AnalyzedContent { get; set; }

        public string Platform { get; set; }
        public ContentType ContentType { get; set; }
        public string TextContent { get; set; }
        public List<string> Hashtags { get; set; }
        public string MediaPath { get; set; }
        public ContentStatus Status { get; set; }
        public DateTime? ScheduledFor { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public virtual PostedContent PostedContent { get; set; }
    }

    public enum ContentType
    {
        Text,
        Image,
        Video,
        Link
    }

    public enum ContentStatus
    {
        Draft,
        PendingApproval,
        Approved,
        Scheduled,
        Posted,
        Failed
    }
}
```

### 1.5 PostedContent.cs

```csharp
using System;
using System.Collections.Generic;

namespace AiSocialPost.Core.Models
{
    public class PostedContent
    {
        public Guid Id { get; set; }
        public Guid GeneratedContentId { get; set; }
        public virtual GeneratedContent GeneratedContent { get; set; }

        public string Platform { get; set; }
        public string PlatformPostId { get; set; }
        public string PostUrl { get; set; }
        public DateTime PostedAt { get; set; }

        // Navigation
        public virtual ICollection<PerformanceMetric> Metrics { get; set; }
    }
}
```

### 1.6 PerformanceMetric.cs

```csharp
using System;

namespace AiSocialPost.Core.Models
{
    public class PerformanceMetric
    {
        public Guid Id { get; set; }
        public Guid PostedContentId { get; set; }
        public virtual PostedContent PostedContent { get; set; }

        public int Likes { get; set; }
        public int Shares { get; set; }
        public int Comments { get; set; }
        public int Views { get; set; }
        public double EngagementRate { get; set; }
        public DateTime CollectedAt { get; set; }
    }
}
```

---

## 2. DATABASE CONTEXT

### AppDbContext.cs

```csharp
using Microsoft.EntityFrameworkCore;
using AiSocialPost.Core.Models;
using System;

namespace AiSocialPost.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<TrendingTopic> TrendingTopics { get; set; }
        public DbSet<ContentSource> ContentSources { get; set; }
        public DbSet<AnalyzedContent> AnalyzedContents { get; set; }
        public DbSet<GeneratedContent> GeneratedContents { get; set; }
        public DbSet<PostedContent> PostedContents { get; set; }
        public DbSet<PerformanceMetric> PerformanceMetrics { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // TrendingTopic configuration
            modelBuilder.Entity<TrendingTopic>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Platform).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Topic).IsRequired();
                entity.Property(e => e.Hashtags)
                    .HasConversion(
                        v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                        v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions)null));
                entity.Property(e => e.Metadata)
                    .HasConversion(
                        v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                        v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions)null));
            });

            // ContentSource configuration
            modelBuilder.Entity<ContentSource>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.TrendingTopic)
                    .WithMany(t => t.ContentSources)
                    .HasForeignKey(e => e.TrendingTopicId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // AnalyzedContent configuration
            modelBuilder.Entity<AnalyzedContent>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Source)
                    .WithOne(s => s.AnalyzedContent)
                    .HasForeignKey<AnalyzedContent>(e => e.SourceId);
                entity.Property(e => e.KeyTopics)
                    .HasConversion(
                        v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                        v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions)null));
                entity.Property(e => e.ViralElements)
                    .HasConversion(
                        v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                        v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions)null));
                entity.Property(e => e.Analysis)
                    .HasConversion(
                        v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                        v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions)null));
            });

            // GeneratedContent configuration
            modelBuilder.Entity<GeneratedContent>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.AnalyzedContent)
                    .WithMany(a => a.GeneratedContents)
                    .HasForeignKey(e => e.AnalyzedContentId);
                entity.Property(e => e.Hashtags)
                    .HasConversion(
                        v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                        v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions)null));
            });

            // PostedContent configuration
            modelBuilder.Entity<PostedContent>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.GeneratedContent)
                    .WithOne(g => g.PostedContent)
                    .HasForeignKey<PostedContent>(e => e.GeneratedContentId);
            });

            // PerformanceMetric configuration
            modelBuilder.Entity<PerformanceMetric>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.PostedContent)
                    .WithMany(p => p.Metrics)
                    .HasForeignKey(e => e.PostedContentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
```

---

## 3. REPOSITORIES

### 3.1 IRepository.cs

```csharp
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AiSocialPost.Data.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(Guid id);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null);
    }
}
```

### 3.2 Repository.cs

```csharp
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AiSocialPost.Data.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate == null)
                return await _dbSet.CountAsync();
            return await _dbSet.CountAsync(predicate);
        }
    }
}
```

---

## 4. SOCIAL MEDIA CLIENTS

### 4.1 Facebook Client

```csharp
using RestSharp;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace AiSocialPost.SocialMedia.Facebook
{
    public class FacebookClient
    {
        private readonly string _accessToken;
        private readonly string _apiVersion = "v22.0";
        private readonly RestClient _client;

        public FacebookClient(string accessToken)
        {
            _accessToken = accessToken;
            _client = new RestClient($"https://graph.facebook.com/{_apiVersion}");
        }

        public async Task<FacebookPostResponse> PostToPageAsync(
            string pageId,
            string message,
            string imageUrl = null,
            string link = null)
        {
            var request = new RestRequest($"/{pageId}/feed", Method.Post);
            request.AddParameter("access_token", _accessToken);
            request.AddParameter("message", message);

            if (!string.IsNullOrEmpty(imageUrl))
                request.AddParameter("picture", imageUrl);

            if (!string.IsNullOrEmpty(link))
                request.AddParameter("link", link);

            var response = await _client.ExecuteAsync(request);

            if (!response.IsSuccessful)
                throw new Exception($"Facebook API Error: {response.Content}");

            return JsonConvert.DeserializeObject<FacebookPostResponse>(response.Content);
        }

        public async Task<FacebookPageFeed> GetPageFeedAsync(string pageId, int limit = 25)
        {
            var request = new RestRequest($"/{pageId}/feed", Method.Get);
            request.AddParameter("access_token", _accessToken);
            request.AddParameter("limit", limit);
            request.AddParameter("fields", "id,message,created_time,likes.summary(true),comments.summary(true),shares");

            var response = await _client.ExecuteAsync(request);

            if (!response.IsSuccessful)
                throw new Exception($"Facebook API Error: {response.Content}");

            return JsonConvert.DeserializeObject<FacebookPageFeed>(response.Content);
        }

        public async Task<FacebookPostMetrics> GetPostMetricsAsync(string postId)
        {
            var request = new RestRequest($"/{postId}", Method.Get);
            request.AddParameter("access_token", _accessToken);
            request.AddParameter("fields",
                "likes.summary(true),comments.summary(true),shares,reactions.summary(true)");

            var response = await _client.ExecuteAsync(request);

            if (!response.IsSuccessful)
                throw new Exception($"Facebook API Error: {response.Content}");

            return JsonConvert.DeserializeObject<FacebookPostMetrics>(response.Content);
        }
    }

    // Response Models
    public class FacebookPostResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class FacebookPageFeed
    {
        [JsonProperty("data")]
        public List<FacebookPost> Data { get; set; }
    }

    public class FacebookPost
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("created_time")]
        public DateTime CreatedTime { get; set; }

        [JsonProperty("likes")]
        public FacebookSummary Likes { get; set; }

        [JsonProperty("comments")]
        public FacebookSummary Comments { get; set; }

        [JsonProperty("shares")]
        public FacebookShares Shares { get; set; }
    }

    public class FacebookSummary
    {
        [JsonProperty("summary")]
        public FacebookSummaryData Summary { get; set; }
    }

    public class FacebookSummaryData
    {
        [JsonProperty("total_count")]
        public int TotalCount { get; set; }
    }

    public class FacebookShares
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }

    public class FacebookPostMetrics
    {
        [JsonProperty("likes")]
        public FacebookSummary Likes { get; set; }

        [JsonProperty("comments")]
        public FacebookSummary Comments { get; set; }

        [JsonProperty("shares")]
        public FacebookShares Shares { get; set; }
    }
}
```

### 4.2 YouTube Client

```csharp
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AiSocialPost.SocialMedia.YouTube
{
    public class YouTubeClient
    {
        private readonly YouTubeService _youtubeService;

        public YouTubeClient(string clientId, string clientSecret, string refreshToken)
        {
            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret
                },
                new[] { YouTubeService.Scope.YoutubeUpload },
                "user",
                CancellationToken.None).Result;

            _youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "AI Social Post"
            });
        }

        public async Task<string> UploadVideoAsync(
            string videoPath,
            string title,
            string description,
            string[] tags,
            string privacyStatus = "public",
            IProgress<IUploadProgress> progress = null)
        {
            var video = new Video
            {
                Snippet = new VideoSnippet
                {
                    Title = title,
                    Description = description,
                    Tags = tags,
                    CategoryId = "22" // People & Blogs
                },
                Status = new VideoStatus
                {
                    PrivacyStatus = privacyStatus
                }
            };

            using (var fileStream = new FileStream(videoPath, FileMode.Open))
            {
                var videosInsertRequest = _youtubeService.Videos.Insert(
                    video,
                    "snippet,status",
                    fileStream,
                    "video/*");

                if (progress != null)
                    videosInsertRequest.ProgressChanged += (p) => progress.Report(p);

                var uploadResult = await videosInsertRequest.UploadAsync();

                if (uploadResult.Status == UploadStatus.Failed)
                    throw new Exception($"Upload failed: {uploadResult.Exception?.Message}");

                return videosInsertRequest.ResponseBody.Id;
            }
        }

        public async Task<VideoStatistics> GetVideoStatisticsAsync(string videoId)
        {
            var request = _youtubeService.Videos.List("statistics");
            request.Id = videoId;

            var response = await request.ExecuteAsync();

            if (response.Items.Count == 0)
                throw new Exception($"Video not found: {videoId}");

            return response.Items[0].Statistics;
        }
    }
}
```

### 4.3 TikTok Client

```csharp
using RestSharp;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AiSocialPost.SocialMedia.TikTok
{
    public class TikTokClient
    {
        private readonly string _accessToken;
        private readonly RestClient _client;

        public TikTokClient(string accessToken)
        {
            _accessToken = accessToken;
            _client = new RestClient("https://open.tiktokapis.com/v2");
        }

        public async Task<TikTokUploadResponse> InitializeUploadAsync(
            string title,
            long videoSize,
            int chunkSize = 10485760) // 10MB chunks
        {
            var request = new RestRequest("/post/publish/video/init/", Method.Post);
            request.AddHeader("Authorization", $"Bearer {_accessToken}");
            request.AddHeader("Content-Type", "application/json");

            var body = new
            {
                post_info = new
                {
                    title = title,
                    privacy_level = "PUBLIC_TO_EVERYONE",
                    disable_duet = false,
                    disable_comment = false,
                    disable_stitch = false,
                    video_cover_timestamp_ms = 1000
                },
                source_info = new
                {
                    source = "FILE_UPLOAD",
                    video_size = videoSize,
                    chunk_size = chunkSize,
                    total_chunk_count = (int)Math.Ceiling((double)videoSize / chunkSize)
                }
            };

            request.AddJsonBody(body);

            var response = await _client.ExecuteAsync(request);

            if (!response.IsSuccessful)
                throw new Exception($"TikTok API Error: {response.Content}");

            return JsonConvert.DeserializeObject<TikTokUploadResponse>(response.Content);
        }

        public async Task UploadVideoChunkAsync(
            string uploadUrl,
            byte[] chunk,
            long start,
            long end,
            long totalSize)
        {
            var client = new RestClient(uploadUrl);
            var request = new RestRequest("", Method.Put);
            request.AddHeader("Content-Type", "video/mp4");
            request.AddHeader("Content-Range", $"bytes {start}-{end}/{totalSize}");
            request.AddParameter("video/mp4", chunk, ParameterType.RequestBody);

            var response = await client.ExecuteAsync(request);

            if (!response.IsSuccessful)
                throw new Exception($"TikTok Upload Error: {response.Content}");
        }

        public async Task<TikTokPublishStatus> PublishVideoAsync(string publishId)
        {
            var request = new RestRequest("/post/publish/status/fetch/", Method.Post);
            request.AddHeader("Authorization", $"Bearer {_accessToken}");
            request.AddHeader("Content-Type", "application/json");

            request.AddJsonBody(new { publish_id = publishId });

            var response = await _client.ExecuteAsync(request);

            if (!response.IsSuccessful)
                throw new Exception($"TikTok API Error: {response.Content}");

            return JsonConvert.DeserializeObject<TikTokPublishStatus>(response.Content);
        }

        public async Task<string> PostVideoAsync(
            string videoPath,
            string title,
            IProgress<double> progress = null)
        {
            // Get file info
            var fileInfo = new FileInfo(videoPath);
            var fileSize = fileInfo.Length;
            const int chunkSize = 10485760; // 10MB

            // Initialize upload
            var initResponse = await InitializeUploadAsync(title, fileSize, chunkSize);

            // Upload chunks
            using (var fileStream = File.OpenRead(videoPath))
            {
                var buffer = new byte[chunkSize];
                long uploadedBytes = 0;

                while (uploadedBytes < fileSize)
                {
                    var bytesRead = await fileStream.ReadAsync(buffer, 0, chunkSize);
                    var chunk = new byte[bytesRead];
                    Array.Copy(buffer, chunk, bytesRead);

                    long end = uploadedBytes + bytesRead - 1;
                    await UploadVideoChunkAsync(
                        initResponse.Data.UploadUrl,
                        chunk,
                        uploadedBytes,
                        end,
                        fileSize);

                    uploadedBytes += bytesRead;
                    progress?.Report((double)uploadedBytes / fileSize * 100);
                }
            }

            // Publish
            var publishResponse = await PublishVideoAsync(initResponse.Data.PublishId);

            return publishResponse.Data.VideoId;
        }
    }

    // Response Models
    public class TikTokUploadResponse
    {
        [JsonProperty("data")]
        public TikTokUploadData Data { get; set; }
    }

    public class TikTokUploadData
    {
        [JsonProperty("publish_id")]
        public string PublishId { get; set; }

        [JsonProperty("upload_url")]
        public string UploadUrl { get; set; }
    }

    public class TikTokPublishStatus
    {
        [JsonProperty("data")]
        public TikTokPublishData Data { get; set; }
    }

    public class TikTokPublishData
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("video_id")]
        public string VideoId { get; set; }
    }
}
```

---

## 5. AI SERVICES

### 5.1 WhisperClient.cs

```csharp
using OpenAI;
using OpenAI.Audio;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AiSocialPost.AI.OpenAI
{
    public class WhisperClient
    {
        private readonly OpenAIClient _client;

        public WhisperClient(string apiKey)
        {
            _client = new OpenAIClient(apiKey);
        }

        public async Task<string> TranscribeAsync(
            string audioFilePath,
            string language = "vi")
        {
            using var audioFile = File.OpenRead(audioFilePath);

            var request = new AudioTranscriptionRequest
            {
                File = audioFile,
                FileName = Path.GetFileName(audioFilePath),
                Model = "whisper-1",
                Language = language,
                ResponseFormat = AudioResponseFormat.Text
            };

            var response = await _client.AudioEndpoint.CreateTranscriptionAsync(request);

            return response;
        }

        public async Task<string> TranscribeVideoAsync(string videoFilePath)
        {
            // Extract audio from video using FFmpeg
            var audioFilePath = await ExtractAudioAsync(videoFilePath);

            try
            {
                return await TranscribeAsync(audioFilePath);
            }
            finally
            {
                // Clean up temp audio file
                if (File.Exists(audioFilePath))
                    File.Delete(audioFilePath);
            }
        }

        private async Task<string> ExtractAudioAsync(string videoFilePath)
        {
            var audioFilePath = Path.Combine(
                Path.GetTempPath(),
                $"{Guid.NewGuid()}.mp3");

            // Using Xabe.FFmpeg
            var conversion = await FFmpeg.Conversions.FromSnippet.ExtractAudio(
                videoFilePath,
                audioFilePath);

            await conversion.Start();

            return audioFilePath;
        }
    }
}
```

### 5.2 ClaudeClient.cs

```csharp
using Anthropic.SDK;
using Anthropic.SDK.Messaging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AiSocialPost.AI.Anthropic
{
    public class ClaudeClient
    {
        private readonly AnthropicClient _client;
        private const string Model = "claude-sonnet-4-5-20250929";

        public ClaudeClient(string apiKey)
        {
            _client = new AnthropicClient(apiKey);
        }

        public async Task<ContentAnalysis> AnalyzeContentAsync(
            string transcript,
            string topic,
            string platform)
        {
            var prompt = $@"Phân tích transcript này và trích xuất:
1. Main topics (5-7 topics chính)
2. Key messages (thông điệp chính)
3. Emotional tone (tông cảm xúc)
4. Viral elements (những yếu tố viral)
5. Target audience (đối tượng mục tiêu)

Trend context: {topic}
Platform: {platform}

Transcript:
{transcript}

Trả về JSON format:
{{
  ""main_topics"": [""topic1"", ""topic2"", ...],
  ""key_messages"": [""message1"", ...],
  ""tone"": ""descriptive|emotional|educational|entertaining"",
  ""viral_elements"": [""element1"", ...],
  ""target_audience"": ""description""
}}";

            var message = await _client.Messages.CreateAsync(new MessageRequest
            {
                Model = Model,
                MaxTokens = 4000,
                Messages = new List<Message>
                {
                    new Message
                    {
                        Role = "user",
                        Content = prompt
                    }
                }
            });

            var responseText = message.Content[0].Text;
            return System.Text.Json.JsonSerializer.Deserialize<ContentAnalysis>(responseText);
        }

        public async Task<string> RewriteForPlatformAsync(
            ContentAnalysis analysis,
            string platform,
            string originalTranscript)
        {
            var platformGuidelines = GetPlatformGuidelines(platform);

            var prompt = $@"Dựa trên analysis này, viết lại content cho {platform}:

Main Topics: {string.Join(", ", analysis.MainTopics)}
Key Messages: {string.Join(", ", analysis.KeyMessages)}
Tone: {analysis.Tone}
Target Audience: {analysis.TargetAudience}

Platform Guidelines:
{platformGuidelines}

Requirements:
1. Giữ main message nhưng viết theo style phù hợp với {platform}
2. Optimize cho engagement
3. Thêm call-to-action phù hợp
4. Gợi ý 5-10 hashtags relevant
5. Độ dài phù hợp với platform

Original transcript (để tham khảo):
{originalTranscript}

Trả về format:
{{
  ""content"": ""nội dung bài viết"",
  ""hashtags"": [""#hashtag1"", ""#hashtag2"", ...],
  ""cta"": ""call to action""
}}";

            var message = await _client.Messages.CreateAsync(new MessageRequest
            {
                Model = Model,
                MaxTokens = 2000,
                Messages = new List<Message>
                {
                    new Message
                    {
                        Role = "user",
                        Content = prompt
                    }
                }
            });

            return message.Content[0].Text;
        }

        private string GetPlatformGuidelines(string platform)
        {
            return platform.ToLower() switch
            {
                "facebook" => @"
- Độ dài: 100-250 từ (có thể dài hơn nếu engaging)
- Style: Conversational, personal
- Format: Paragraphs với spacing tốt
- Call-to-action: Rõ ràng (like, share, comment)
- Hashtags: 2-5 hashtags",

                "tiktok" => @"
- Độ dài: Ngắn gọn, 50-150 từ
- Style: Trendy, fun, có hook
- Format: Short sentences, emojis
- Call-to-action: Like, follow, duet
- Hashtags: 5-10 hashtags (bao gồm trending)",

                "youtube" => @"
- Độ dài: 200-500 từ cho description
- Style: Informative, SEO-friendly
- Format: Structured với timestamps nếu cần
- Call-to-action: Subscribe, like, notification bell
- Hashtags: 3-5 hashtags ở cuối",

                "zalo" => @"
- Độ dài: 100-200 từ
- Style: Formal hơn, professional
- Format: Clean, structured
- Call-to-action: Clear action button text
- Hashtags: 1-3 hashtags",

                _ => "Viết content engaging và phù hợp"
            };
        }
    }

    public class ContentAnalysis
    {
        [System.Text.Json.Serialization.JsonPropertyName("main_topics")]
        public List<string> MainTopics { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("key_messages")]
        public List<string> KeyMessages { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("tone")]
        public string Tone { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("viral_elements")]
        public List<string> ViralElements { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("target_audience")]
        public string TargetAudience { get; set; }
    }
}
```

---

## 6. BUSINESS SERVICES

### 6.1 TrendDetectionService.cs

```csharp
using AiSocialPost.Core.Models;
using AiSocialPost.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiSocialPost.Core.Services
{
    public class TrendDetectionService
    {
        private readonly IRepository<TrendingTopic> _trendRepository;
        // Add trend API clients here

        public TrendDetectionService(IRepository<TrendingTopic> trendRepository)
        {
            _trendRepository = trendRepository;
        }

        public async Task<List<TrendingTopic>> DetectTrendsAsync()
        {
            var trends = new List<TrendingTopic>();

            // Fetch from multiple sources
            var tiktokTrends = await FetchTikTokTrendsAsync();
            var googleTrends = await FetchGoogleTrendsAsync();
            var youtubeTrends = await FetchYouTubeTrendsAsync();

            trends.AddRange(tiktokTrends);
            trends.AddRange(googleTrends);
            trends.AddRange(youtubeTrends);

            // Score and filter
            var scoredTrends = ScoreTrends(trends);
            var hotTrends = scoredTrends
                .Where(t => t.EngagementScore > 1000)
                .OrderByDescending(t => t.EngagementScore)
                .ToList();

            // Save to database
            foreach (var trend in hotTrends)
            {
                // Check if already exists
                var existing = await _trendRepository.FindAsync(
                    t => t.Topic == trend.Topic && t.Platform == trend.Platform);

                if (!existing.Any())
                {
                    await _trendRepository.AddAsync(trend);
                }
                else
                {
                    // Update score
                    var existingTrend = existing.First();
                    existingTrend.EngagementScore = trend.EngagementScore;
                    existingTrend.Status = DetermineStatus(existingTrend, trend);
                    if (trend.EngagementScore > existingTrend.EngagementScore)
                    {
                        existingTrend.PeakTime = DateTime.UtcNow;
                    }
                    await _trendRepository.UpdateAsync(existingTrend);
                }
            }

            return hotTrends;
        }

        private async Task<List<TrendingTopic>> FetchTikTokTrendsAsync()
        {
            // Implementation using TikTok Trends API
            // This is placeholder - actual implementation needed
            return new List<TrendingTopic>();
        }

        private async Task<List<TrendingTopic>> FetchGoogleTrendsAsync()
        {
            // Implementation using Google Trends API
            return new List<TrendingTopic>();
        }

        private async Task<List<TrendingTopic>> FetchYouTubeTrendsAsync()
        {
            // Implementation using YouTube Trending API
            return new List<TrendingTopic>();
        }

        private List<TrendingTopic> ScoreTrends(List<TrendingTopic> trends)
        {
            // Scoring algorithm
            foreach (var trend in trends)
            {
                // Calculate engagement score based on various factors
                // This is simplified - actual implementation would be more complex
                trend.EngagementScore = CalculateEngagementScore(trend);
            }

            return trends;
        }

        private int CalculateEngagementScore(TrendingTopic trend)
        {
            // Placeholder scoring logic
            // Real implementation would consider:
            // - Number of posts
            // - Growth velocity
            // - Cross-platform presence
            // - Engagement rates
            return new Random().Next(100, 10000);
        }

        private TrendStatus DetermineStatus(TrendingTopic existing, TrendingTopic current)
        {
            if (current.EngagementScore > existing.EngagementScore * 1.5)
                return TrendStatus.Rising;
            if (current.EngagementScore < existing.EngagementScore * 0.7)
                return TrendStatus.Declining;
            return TrendStatus.Peak;
        }
    }
}
```

---

## 7. WINFORMS UI

### 7.1 MainForm.cs

```csharp
using System;
using System.Windows.Forms;
using AiSocialPost.Core.Services;

namespace AiSocialPost.WinForms.Forms
{
    public partial class MainForm : Form
    {
        private readonly TrendDetectionService _trendService;
        private readonly ContentAnalysisService _contentService;
        private readonly ContentGenerationService _generationService;
        private readonly SchedulingService _schedulingService;

        private TrendingForm _trendingForm;
        private ContentForm _contentForm;
        private ScheduleForm _scheduleForm;
        private AnalyticsForm _analyticsForm;

        public MainForm(
            TrendDetectionService trendService,
            ContentAnalysisService contentService,
            ContentGenerationService generationService,
            SchedulingService schedulingService)
        {
            InitializeComponent();

            _trendService = trendService;
            _contentService = contentService;
            _generationService = generationService;
            _schedulingService = schedulingService;

            InitializeChildForms();
            SetupEventHandlers();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 800);
            this.Text = "AI Social Post - Hệ Thống Đăng Bài Tự Động";
            this.IsMdiContainer = true;

            // Menu Strip
            var menuStrip = new MenuStrip();

            // File Menu
            var fileMenu = new ToolStripMenuItem("&File");
            fileMenu.DropDownItems.Add("&Settings", null, Settings_Click);
            fileMenu.DropDownItems.Add(new ToolStripSeparator());
            fileMenu.DropDownItems.Add("E&xit", null, Exit_Click);
            menuStrip.Items.Add(fileMenu);

            // View Menu
            var viewMenu = new ToolStripMenuItem("&View");
            viewMenu.DropDownItems.Add("&Trending", null, ShowTrending_Click);
            viewMenu.DropDownItems.Add("&Content", null, ShowContent_Click);
            viewMenu.DropDownItems.Add("&Schedule", null, ShowSchedule_Click);
            viewMenu.DropDownItems.Add("&Analytics", null, ShowAnalytics_Click);
            menuStrip.Items.Add(viewMenu);

            // Tools Menu
            var toolsMenu = new ToolStripMenuItem("&Tools");
            toolsMenu.DropDownItems.Add("&Detect Trends Now", null, DetectTrends_Click);
            toolsMenu.DropDownItems.Add("&Social Media Accounts", null, SocialAccounts_Click);
            menuStrip.Items.Add(toolsMenu);

            this.Controls.Add(menuStrip);
            this.MainMenuStrip = menuStrip;

            // Status Strip
            var statusStrip = new StatusStrip();
            var statusLabel = new ToolStripStatusLabel("Ready");
            statusStrip.Items.Add(statusLabel);
            this.Controls.Add(statusStrip);

            // Toolbar
            var toolStrip = new ToolStrip();
            toolStrip.Items.Add("Trending", null, ShowTrending_Click);
            toolStrip.Items.Add("Content", null, ShowContent_Click);
            toolStrip.Items.Add("Schedule", null, ShowSchedule_Click);
            toolStrip.Items.Add("Analytics", null, ShowAnalytics_Click);
            this.Controls.Add(toolStrip);
        }

        private void InitializeChildForms()
        {
            _trendingForm = new TrendingForm(_trendService);
            _trendingForm.MdiParent = this;

            _contentForm = new ContentForm(_contentService, _generationService);
            _contentForm.MdiParent = this;

            _scheduleForm = new ScheduleForm(_schedulingService);
            _scheduleForm.MdiParent = this;

            _analyticsForm = new AnalyticsForm();
            _analyticsForm.MdiParent = this;
        }

        private void SetupEventHandlers()
        {
            this.Load += MainForm_Load;
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            // Show trending form by default
            ShowTrending_Click(sender, e);

            // Start background trend detection
            await StartTrendMonitoring();
        }

        private async Task StartTrendMonitoring()
        {
            // Setup timer for periodic trend detection (every 15 minutes)
            var timer = new System.Windows.Forms.Timer();
            timer.Interval = 15 * 60 * 1000; // 15 minutes
            timer.Tick += async (s, e) =>
            {
                try
                {
                    await _trendService.DetectTrendsAsync();
                    _trendingForm?.RefreshTrends();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error detecting trends: {ex.Message}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            timer.Start();
        }

        private void ShowTrending_Click(object sender, EventArgs e)
        {
            _trendingForm.Show();
            _trendingForm.BringToFront();
        }

        private void ShowContent_Click(object sender, EventArgs e)
        {
            _contentForm.Show();
            _contentForm.BringToFront();
        }

        private void ShowSchedule_Click(object sender, EventArgs e)
        {
            _scheduleForm.Show();
            _scheduleForm.BringToFront();
        }

        private void ShowAnalytics_Click(object sender, EventArgs e)
        {
            _analyticsForm.Show();
            _analyticsForm.BringToFront();
        }

        private async void DetectTrends_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                await _trendService.DetectTrendsAsync();
                _trendingForm?.RefreshTrends();
                MessageBox.Show("Trends detected successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void SocialAccounts_Click(object sender, EventArgs e)
        {
            // Open social media accounts management form
            var accountsForm = new SocialAccountsForm();
            accountsForm.ShowDialog(this);
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            // Open settings form
            var settingsForm = new SettingsForm();
            settingsForm.ShowDialog(this);
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
```

---

## 8. DEPENDENCY INJECTION SETUP

### Program.cs

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Windows.Forms;
using AiSocialPost.Data;
using AiSocialPost.Data.Repositories;
using AiSocialPost.Core.Services;
using AiSocialPost.SocialMedia.Facebook;
using AiSocialPost.SocialMedia.YouTube;
using AiSocialPost.SocialMedia.TikTok;
using AiSocialPost.AI.OpenAI;
using AiSocialPost.AI.Anthropic;

namespace AiSocialPost.WinForms
{
    static class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Setup configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config/appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Setup DI
            var services = new ServiceCollection();
            ConfigureServices(services, configuration);
            ServiceProvider = services.BuildServiceProvider();

            // Run application
            var mainForm = ServiceProvider.GetRequiredService<MainForm>();
            Application.Run(mainForm);
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Configuration
            services.AddSingleton<IConfiguration>(configuration);

            // Database
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString)); // or UseSqlite

            // Repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Core Services
            services.AddScoped<TrendDetectionService>();
            services.AddScoped<ContentAnalysisService>();
            services.AddScoped<ContentGenerationService>();
            services.AddScoped<SchedulingService>();
            services.AddScoped<AnalyticsService>();

            // Social Media Clients
            services.AddScoped<FacebookClient>(sp =>
            {
                var accessToken = configuration["ApiKeys:Facebook:PageAccessToken"];
                return new FacebookClient(accessToken);
            });

            services.AddScoped<YouTubeClient>(sp =>
            {
                var clientId = configuration["ApiKeys:YouTube:ClientId"];
                var clientSecret = configuration["ApiKeys:YouTube:ClientSecret"];
                var refreshToken = configuration["ApiKeys:YouTube:RefreshToken"];
                return new YouTubeClient(clientId, clientSecret, refreshToken);
            });

            services.AddScoped<TikTokClient>(sp =>
            {
                var accessToken = configuration["ApiKeys:TikTok:AccessToken"];
                return new TikTokClient(accessToken);
            });

            // AI Clients
            services.AddScoped<WhisperClient>(sp =>
            {
                var apiKey = configuration["ApiKeys:OpenAI:ApiKey"];
                return new WhisperClient(apiKey);
            });

            services.AddScoped<ClaudeClient>(sp =>
            {
                var apiKey = configuration["ApiKeys:Anthropic:ApiKey"];
                return new ClaudeClient(apiKey);
            });

            // Forms
            services.AddTransient<MainForm>();
            services.AddTransient<TrendingForm>();
            services.AddTransient<ContentForm>();
            services.AddTransient<ScheduleForm>();
            services.AddTransient<AnalyticsForm>();
        }
    }
}
```

---

Đây là foundation cho C# WinForms application. Các phần còn lại (TrendingForm, ContentForm, etc.) sẽ được implement tương tự với UI controls phù hợp.

Bạn có muốn tôi tiếp tục implement các form cụ thể không?
