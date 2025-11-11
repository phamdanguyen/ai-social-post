# Gemini-First AI Strategy

## Tổng Quan

**Philosophy: KISS (Keep It Simple, Stupid)**

Thay vì integrate nhiều AI providers (Meta AI, Grok, ChatGPT, Claude, Whisper, DALL-E), chúng ta sử dụng **Google Gemini làm chủ đạo** cho hầu hết tasks.

### Tại Sao Gemini?

✅ **All-in-One Solution**
- Text generation (như GPT-4)
- Image understanding (multimodal)
- Search integration (real-time data)
- Video understanding (analyze videos)
- Code generation
- Vietnamese language support

✅ **Cost-Effective**
- Gemini Flash: $0.075/$0.30 per 1M tokens (cheap!)
- Gemini Pro: $1.25/$5 per 1M tokens (competitive)
- Free tier: 15 requests/min

✅ **Google Ecosystem**
- YouTube integration native
- Google Trends integration
- Google Search grounding
- Veo 3 for video generation
- ImageFX for images

✅ **Simplicity**
- One API key
- One SDK
- Consistent interface
- Easy to maintain

---

## 1. SIMPLIFIED AI STACK

### Before (Complex)

```
Trend Detection:
├─ Meta AI (Llama) for Facebook trends
├─ Grok for Twitter/X trends
├─ ChatGPT for multi-platform
└─ Google Trends API

Content Analysis:
├─ OpenAI Whisper for transcription
├─ Claude for analysis
└─ GPT-4 for generation

Media Generation:
├─ DALL-E for images
└─ Runway ML for videos

Total: 8+ services, 8+ API keys, complex integration
```

### After (Simple)

```
🎯 **Google Gemini** - Primary AI
├─ Trend detection (with search grounding)
├─ Content analysis (multimodal)
├─ Content generation (text)
├─ Video transcription (built-in)
└─ Video understanding

📸 **Banana API** - Image Generation
└─ Generate images from text

🎬 **Veo 3** - Video Generation
└─ Generate videos from text/images

Total: 3 services, 3 API keys, simple integration
```

---

## 2. GEMINI CAPABILITIES

### 2.1 Model Lineup

| Model | Use Case | Cost | Speed |
|-------|----------|------|-------|
| **Gemini 2.0 Flash** | Fast tasks, real-time | $0.075/$0.30 per 1M tokens | ⚡⚡⚡ |
| **Gemini 1.5 Pro** | Complex analysis | $1.25/$5 per 1M tokens | ⚡⚡ |
| **Gemini 1.5 Flash** | Balanced | $0.075/$0.30 per 1M tokens | ⚡⚡⚡ |

**Recommendation:**
- **Trend Detection**: Gemini 2.0 Flash (fast, cheap)
- **Content Analysis**: Gemini 1.5 Pro (better understanding)
- **Content Generation**: Gemini 2.0 Flash (fast, good enough)
- **Comment Replies**: Gemini 2.0 Flash (real-time)

### 2.2 Key Features

#### Multimodal Input
```csharp
// Analyze image + text
var prompt = "Analyze this viral TikTok video and extract key elements";
var response = await gemini.GenerateContentAsync(
    prompt,
    videoFile: "viral_video.mp4"
);
```

#### Search Grounding (Real-Time Data)
```csharp
// Get current trends with real data
var prompt = "What are the top trending topics on Vietnamese social media today?";
var response = await gemini.GenerateContentAsync(
    prompt,
    tools: new[] { Tool.GoogleSearch }
);
// Gemini searches web and provides current data!
```

#### Long Context (2M tokens!)
```csharp
// Analyze entire video transcript
var prompt = "Summarize this 2-hour podcast";
var response = await gemini.GenerateContentAsync(
    prompt,
    videoFile: "podcast.mp4"
);
```

#### Function Calling
```csharp
// Gemini can call your functions
var functions = new[]
{
    new Function("post_to_facebook", "Post content to Facebook page"),
    new Function("get_post_metrics", "Get metrics for a post")
};

var response = await gemini.GenerateContentAsync(
    "Post this content to Facebook and tell me how it's performing",
    tools: functions
);
```

---

## 3. IMPLEMENTATION STRATEGY

### 3.1 Unified AI Service

```csharp
public class GeminiAIService
{
    private readonly GenerativeModel _flashModel;    // Fast & cheap
    private readonly GenerativeModel _proModel;      // Powerful
    private readonly IConfiguration _config;

    public GeminiAIService(IConfiguration config)
    {
        _config = config;
        var apiKey = config["Google:ApiKey"];

        _flashModel = new GenerativeModel(
            apiKey: apiKey,
            model: "gemini-2.0-flash",
            generationConfig: new GenerationConfig
            {
                Temperature = 0.7f,
                TopP = 0.95f,
                MaxOutputTokens = 8192
            }
        );

        _proModel = new GenerativeModel(
            apiKey: apiKey,
            model: "gemini-1.5-pro",
            generationConfig: new GenerationConfig
            {
                Temperature = 0.7f,
                TopP = 0.95f,
                MaxOutputTokens = 8192
            }
        );
    }

    // Use cases below...
}
```

---

### 3.2 Trend Detection with Gemini

```csharp
public class GeminiTrendDetector
{
    private readonly GeminiAIService _gemini;

    public async Task<List<TrendingTopic>> DetectTrendsAsync(string region = "Vietnam")
    {
        var prompt = $@"
You are a social media trend analyst for {region}.

Use Google Search to find what's currently trending today across:
- Facebook
- Instagram
- TikTok
- YouTube
- Twitter/X

For each trend, provide:
1. Topic name
2. Platform(s) where it's trending
3. Why it's trending (brief explanation)
4. Estimated engagement level (High/Medium/Low)
5. Trend status (Rising/Peak/Declining)
6. Suggested hashtags
7. Target audience
8. Content ideas

Return as JSON array with this structure:
{{
  ""trends"": [
    {{
      ""topic"": ""..."",
      ""platforms"": [""Facebook"", ""TikTok""],
      ""reason"": ""..."",
      ""engagement"": ""High"",
      ""status"": ""Rising"",
      ""hashtags"": [""#tag1"", ""#tag2""],
      ""audience"": ""..."",
      ""content_ideas"": [""idea1"", ""idea2""]
    }}
  ]
}}

Focus on trends from the last 24 hours.
Prioritize trends relevant for brands/creators.
";

        var response = await _gemini.GenerateWithSearchAsync(prompt);
        var trendsData = ParseTrendsFromJson(response.Text);

        return trendsData.Select(t => new TrendingTopic
        {
            Id = Guid.NewGuid(),
            Topic = t.Topic,
            Platform = string.Join(", ", t.Platforms),
            Hashtags = t.Hashtags,
            Score = CalculateScore(t.Engagement, t.Status),
            Status = ParseStatus(t.Status),
            DetectedAt = DateTime.UtcNow,
            AnalysisJson = JsonSerializer.Serialize(t)
        }).ToList();
    }

    private async Task<GeminiResponse> GenerateWithSearchAsync(string prompt)
    {
        return await _flashModel.GenerateContentAsync(
            prompt,
            tools: new[] { Tool.GoogleSearch }
        );
    }
}
```

---

### 3.3 Video Analysis with Gemini

```csharp
public class GeminiVideoAnalyzer
{
    private readonly GeminiAIService _gemini;

    public async Task<VideoAnalysis> AnalyzeViralVideoAsync(string videoUrl)
    {
        // Step 1: Download video
        var videoFile = await DownloadVideoAsync(videoUrl);

        // Step 2: Analyze with Gemini (multimodal)
        var prompt = @"
Analyze this viral video and extract:

1. **Visual Elements:**
   - Key scenes and moments
   - Visual hooks (first 3 seconds)
   - Color palette and aesthetics
   - Text overlays and captions

2. **Audio Elements:**
   - Background music/sound
   - Voiceover or dialogue (transcribe)
   - Audio hooks

3. **Content Structure:**
   - Opening hook
   - Main content flow
   - Call-to-action
   - Duration and pacing

4. **Viral Elements:**
   - What makes it shareable?
   - Emotional triggers
   - Trending elements
   - Unique angles

5. **Recreating Strategy:**
   - How to create similar content
   - What to change to avoid copyright
   - Platform-specific adaptations

Return as detailed JSON.";

        var response = await _proModel.GenerateContentAsync(
            prompt,
            videoFile: videoFile
        );

        return ParseVideoAnalysis(response.Text);
    }

    public async Task<string> TranscribeVideoAsync(string videoUrl)
    {
        var videoFile = await DownloadVideoAsync(videoUrl);

        var prompt = "Transcribe all speech in this video. Include timestamps.";

        var response = await _flashModel.GenerateContentAsync(
            prompt,
            videoFile: videoFile
        );

        return response.Text;
    }
}
```

**No need for Whisper!** Gemini can transcribe directly.

---

### 3.4 Content Generation with Gemini

```csharp
public class GeminiContentGenerator
{
    private readonly GeminiAIService _gemini;
    private readonly BrandVoice _brandVoice;

    public async Task<Dictionary<string, GeneratedContent>> GenerateMultiPlatformContentAsync(
        VideoAnalysis analysis,
        TrendingTopic trend)
    {
        var platforms = new[] { "facebook", "instagram", "tiktok", "youtube", "twitter", "zalo" };
        var results = new Dictionary<string, GeneratedContent>();

        foreach (var platform in platforms)
        {
            var content = await GenerateForPlatformAsync(analysis, trend, platform);
            results[platform] = content;
        }

        return results;
    }

    private async Task<GeneratedContent> GenerateForPlatformAsync(
        VideoAnalysis analysis,
        TrendingTopic trend,
        string platform)
    {
        var prompt = $@"
Based on this viral video analysis, create content for {platform}.

**Original Analysis:**
{JsonSerializer.Serialize(analysis)}

**Trending Topic:**
{trend.Topic}

**Brand Voice:**
- Tone: {_brandVoice.Tone}
- Style: {_brandVoice.Style}
- Language: Vietnamese

**Platform Guidelines for {platform}:**
{GetPlatformGuidelines(platform)}

**Task:**
Create engaging content that:
1. Captures the viral elements from analysis
2. Adapts format for {platform}
3. Matches our brand voice
4. Includes relevant hashtags
5. Has strong hook and CTA

Return JSON:
{{
  ""text_content"": ""...(post caption)"",
  ""hashtags"": [""#tag1"", ""#tag2""],
  ""cta"": ""...(call to action)"",
  ""image_prompt"": ""...(for Banana API)"",
  ""video_concept"": ""...(for Veo 3)""
}}
";

        var response = await _flashModel.GenerateContentAsync(prompt);
        var generated = JsonSerializer.Deserialize<GeneratedContentJson>(response.Text);

        return new GeneratedContent
        {
            Platform = platform,
            TextContent = generated.TextContent,
            Hashtags = generated.Hashtags,
            ImagePrompt = generated.ImagePrompt,
            VideoPrompt = generated.VideoConcept
        };
    }
}
```

---

### 3.5 Comment Reply with Gemini

```csharp
public class GeminiCommentReplyService
{
    private readonly GeminiAIService _gemini;

    public async Task<CommentReply> GenerateReplyAsync(Comment comment, Post postContext)
    {
        // Step 1: Analyze sentiment and intent
        var analysis = await AnalyzeCommentAsync(comment.Text);

        // Step 2: Generate appropriate reply
        if (analysis.RequiresHuman)
        {
            return new CommentReply { EscalateToHuman = true, Reason = analysis.Reason };
        }

        var prompt = $@"
You are managing social media for {_brandVoice.BrandName}.

**Post Context:**
{postContext.Content}

**User Comment:**
""{comment.Text}""

**Comment Analysis:**
- Sentiment: {analysis.Sentiment}
- Intent: {analysis.Intent}
- Language: {analysis.Language}

**Brand Voice:**
- Tone: {_brandVoice.Tone}
- DOs: {string.Join(", ", _brandVoice.Dos)}
- DON'Ts: {string.Join(", ", _brandVoice.Donts)}

**Task:**
Generate a natural, engaging reply that:
1. Addresses user's {analysis.Intent}
2. Matches our brand voice
3. Is concise (< 100 words)
4. Encourages engagement
5. Written in {analysis.Language}

{GetIntentSpecificGuidelines(analysis.Intent)}

Reply directly without explanation:";

        var response = await _flashModel.GenerateContentAsync(prompt);

        return new CommentReply
        {
            Text = response.Text.Trim(),
            ShouldPost = true
        };
    }

    private async Task<CommentAnalysis> AnalyzeCommentAsync(string commentText)
    {
        var prompt = $@"
Analyze this social media comment:
""{commentText}""

Provide:
{{
  ""sentiment"": ""Positive/Negative/Neutral/Mixed"",
  ""intent"": ""Question/Inquiry/Compliment/Complaint/Spam/Other"",
  ""language"": ""vi/en/..."",
  ""requires_human"": {{
    ""value"": true/false,
    ""reason"": ""...""
  }},
  ""urgency"": ""Low/Medium/High""
}}

Escalate to human if:
- Negative complaint
- Legal/refund issues
- Aggressive language
- Complex questions
";

        var response = await _flashModel.GenerateContentAsync(prompt);
        return JsonSerializer.Deserialize<CommentAnalysis>(response.Text);
    }
}
```

---

## 4. BANANA API FOR IMAGES

### 4.1 What is Banana?

**Banana.dev** - Serverless GPU infrastructure for AI models
- Fast inference
- Scalable
- Cost-effective
- Pre-built models (Stable Diffusion, FLUX, etc.)

### 4.2 Integration

```csharp
public class BananaImageGenerator
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _modelKey;

    public BananaImageGenerator(string apiKey, string modelKey)
    {
        _apiKey = apiKey;
        _modelKey = modelKey;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.banana.dev")
        };
    }

    public async Task<string> GenerateImageAsync(string prompt, ImageStyle style = ImageStyle.Realistic)
    {
        var requestBody = new
        {
            apiKey = _apiKey,
            modelKey = _modelKey,
            modelInputs = new
            {
                prompt = EnhancePrompt(prompt, style),
                negative_prompt = "blurry, low quality, distorted, watermark",
                num_inference_steps = 30,
                guidance_scale = 7.5,
                width = 1024,
                height = 1024
            }
        };

        var response = await _httpClient.PostAsJsonAsync("/", requestBody);
        var result = await response.Content.ReadFromJsonAsync<BananaResponse>();

        if (result.ModelOutputs == null || result.ModelOutputs.Length == 0)
            throw new Exception("Image generation failed");

        // Image returned as base64
        var imageBase64 = result.ModelOutputs[0].ImageBase64;

        // Save to storage
        var imageUrl = await SaveToStorageAsync(imageBase64);

        return imageUrl;
    }

    private string EnhancePrompt(string userPrompt, ImageStyle style)
    {
        var styleModifiers = style switch
        {
            ImageStyle.Realistic => "photorealistic, high quality, detailed, 8k",
            ImageStyle.Artistic => "artistic, creative, vibrant colors, stylized",
            ImageStyle.Minimal => "minimalist, clean, simple, modern",
            ImageStyle.Cartoon => "cartoon style, illustrated, colorful, fun",
            _ => ""
        };

        return $"{userPrompt}, {styleModifiers}";
    }

    private async Task<string> SaveToStorageAsync(string base64Image)
    {
        var bytes = Convert.FromBase64String(base64Image);
        var fileName = $"{Guid.NewGuid()}.png";

        // Upload to S3/Azure Blob/etc
        var url = await _storageService.UploadAsync(fileName, bytes, "image/png");

        return url;
    }
}

public enum ImageStyle
{
    Realistic,
    Artistic,
    Minimal,
    Cartoon
}
```

### 4.3 Usage in Content Generation

```csharp
// In ContentGenerationService
public async Task GenerateCompleteContentAsync(GeneratedContent content)
{
    // 1. Generate text with Gemini (already done)

    // 2. Generate image with Banana
    if (!string.IsNullOrEmpty(content.ImagePrompt))
    {
        var imageUrl = await _bananaImageGen.GenerateImageAsync(
            content.ImagePrompt,
            ImageStyle.Realistic
        );

        content.MediaUrl = imageUrl;
    }

    // 3. Generate video with Veo 3 (if needed)
    if (!string.IsNullOrEmpty(content.VideoPrompt))
    {
        var videoUrl = await _veoVideoGen.GenerateVideoAsync(
            content.VideoPrompt,
            duration: 15 // seconds
        );

        content.MediaUrl = videoUrl;
    }
}
```

---

## 5. VEO 3 FOR VIDEO GENERATION

### 5.1 What is Veo 3?

**Google Veo 3** - State-of-the-art video generation model
- Text-to-video
- Image-to-video
- High quality 1080p
- Up to 2 minutes
- Realistic motion

### 5.2 Integration (via Google AI Studio)

```csharp
public class Veo3VideoGenerator
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public async Task<string> GenerateVideoAsync(
        string prompt,
        int duration = 15,
        string aspectRatio = "16:9")
    {
        var requestBody = new
        {
            prompt = prompt,
            duration = duration,
            aspect_ratio = aspectRatio,
            fps = 30,
            quality = "high"
        };

        var response = await _httpClient.PostAsJsonAsync(
            "https://generativelanguage.googleapis.com/v1/models/veo-3:generateVideo",
            requestBody
        );

        var result = await response.Content.ReadFromJsonAsync<Veo3Response>();

        // Poll for completion (video generation takes time)
        var videoUrl = await PollForCompletionAsync(result.JobId);

        return videoUrl;
    }

    private async Task<string> PollForCompletionAsync(string jobId)
    {
        while (true)
        {
            var status = await CheckJobStatusAsync(jobId);

            if (status.Status == "completed")
                return status.VideoUrl;

            if (status.Status == "failed")
                throw new Exception($"Video generation failed: {status.Error}");

            await Task.Delay(5000); // Check every 5 seconds
        }
    }

    public async Task<string> GenerateVideoFromImageAsync(
        string imageUrl,
        string motionPrompt,
        int duration = 10)
    {
        var requestBody = new
        {
            input_image = imageUrl,
            motion_prompt = motionPrompt,
            duration = duration
        };

        var response = await _httpClient.PostAsJsonAsync(
            "https://generativelanguage.googleapis.com/v1/models/veo-3:generateVideoFromImage",
            requestBody
        );

        var result = await response.Content.ReadFromJsonAsync<Veo3Response>();
        return await PollForCompletionAsync(result.JobId);
    }
}
```

### 5.3 Video Generation Workflow

```csharp
public async Task CreateVideoContentAsync(GeneratedContent content)
{
    if (content.Platform == "tiktok" || content.Platform == "youtube")
    {
        // Option 1: Text-to-video
        var videoUrl = await _veo3.GenerateVideoAsync(
            prompt: content.VideoPrompt,
            duration: content.Platform == "tiktok" ? 15 : 60
        );

        content.MediaUrl = videoUrl;

        // OR

        // Option 2: Image-to-video (more control)
        // First generate image
        var imageUrl = await _banana.GenerateImageAsync(content.ImagePrompt);

        // Then animate it
        var videoUrl = await _veo3.GenerateVideoFromImageAsync(
            imageUrl,
            motionPrompt: "Smooth pan across the scene, professional cinematography",
            duration: 10
        );

        content.MediaUrl = videoUrl;
    }
}
```

---

## 6. COST COMPARISON

### Before (Multi-AI Stack)

| Service | Monthly Cost |
|---------|-------------|
| OpenAI GPT-4 | $450 |
| OpenAI Whisper | $90 |
| OpenAI DALL-E | $60 |
| Anthropic Claude | $200 |
| Grok API | $50 |
| **Total** | **$850/month** |

### After (Gemini-First)

| Service | Monthly Cost |
|---------|-------------|
| Google Gemini | $150 (Flash: $50, Pro: $100) |
| Banana Images | $100 (1000 images) |
| Veo 3 Videos | $200 (100 videos) |
| **Total** | **$450/month** |

**Savings: $400/month (47% cheaper!)**

Plus:
- ✅ Simpler codebase
- ✅ Fewer dependencies
- ✅ Easier maintenance
- ✅ One primary vendor

---

## 7. IMPLEMENTATION TIMELINE

### Week 1-2: Gemini Foundation
- Setup Google AI SDK
- Implement GeminiAIService
- Test basic prompts
- Trend detection with search

### Week 3-4: Content Generation
- Video analysis (multimodal)
- Content generation per platform
- Hashtag suggestions
- Brand voice integration

### Week 5-6: Media Generation
- Banana API integration
- Image generation pipeline
- Veo 3 setup (when available)
- Storage management

### Week 7-8: Engagement
- Comment reply automation
- Sentiment analysis
- Lead capture logic
- Quality checks

**Total: 8 weeks for complete Gemini-first implementation**

---

## 8. CODE STRUCTURE

```
src/AiSocialPost.AI/
├── Gemini/
│   ├── GeminiAIService.cs          # Core service
│   ├── GeminiTrendDetector.cs      # Trend detection
│   ├── GeminiVideoAnalyzer.cs      # Video analysis
│   ├── GeminiContentGenerator.cs   # Content gen
│   ├── GeminiCommentReply.cs       # Comment replies
│   └── GeminiConfig.cs             # Configuration
│
├── Banana/
│   ├── BananaImageGenerator.cs     # Image generation
│   └── BananaConfig.cs
│
├── Veo/
│   ├── Veo3VideoGenerator.cs       # Video generation
│   └── Veo3Config.cs
│
└── Interfaces/
    ├── IAIService.cs
    ├── IImageGenerator.cs
    └── IVideoGenerator.cs
```

---

## 9. CONFIGURATION

### appsettings.json

```json
{
  "Google": {
    "ApiKey": "your-google-api-key",
    "Models": {
      "Flash": "gemini-2.0-flash",
      "Pro": "gemini-1.5-pro"
    },
    "RateLimits": {
      "RequestsPerMinute": 15,
      "RequestsPerDay": 1500
    }
  },
  "Banana": {
    "ApiKey": "your-banana-api-key",
    "ModelKey": "stable-diffusion-xl",
    "Endpoint": "https://api.banana.dev"
  },
  "Veo": {
    "ApiKey": "your-veo-api-key",
    "Endpoint": "https://generativelanguage.googleapis.com/v1"
  },
  "BrandVoice": {
    "BrandName": "Your Brand",
    "Tone": "Friendly and Professional",
    "Style": "Conversational",
    "Language": "Vietnamese",
    "Dos": [
      "Use emojis sparingly",
      "Be helpful and informative",
      "Engage naturally"
    ],
    "Donts": [
      "No aggressive language",
      "No promises we can't keep",
      "No spam"
    ]
  }
}
```

---

## 10. ADVANTAGES OF GEMINI-FIRST

### Technical Advantages

✅ **Single Integration Point**
- One SDK to learn
- One authentication flow
- Consistent error handling

✅ **Better Context**
- 2M token context window
- Can analyze entire videos
- Remember conversation history

✅ **Real-Time Data**
- Google Search integration
- Current trends, not stale data
- Fact-checking built-in

✅ **Multimodal Native**
- No need for separate transcription
- Analyze images + videos + text together
- Better understanding

### Business Advantages

✅ **Cost Savings**
- 47% cheaper than multi-AI
- Predictable pricing
- Free tier for testing

✅ **Faster Development**
- Fewer integrations
- Simpler codebase
- Easier testing

✅ **Easier Maintenance**
- One vendor relationship
- One set of docs
- Fewer breaking changes

✅ **Better Reliability**
- Google's infrastructure
- 99.9% uptime
- Global CDN

---

## 11. LIMITATIONS & WORKAROUNDS

### Limitation 1: Rate Limits (15 req/min free tier)

**Workaround:**
- Use batch processing
- Cache results
- Upgrade to paid tier ($0.20/1M tokens)

### Limitation 2: Video Generation (Veo 3 may not be public yet)

**Workaround:**
- Use Runway ML or Pika as backup
- Focus on images first
- Wait for Veo 3 public release

### Limitation 3: Image Quality (Banana depends on model)

**Workaround:**
- Test multiple models (SDXL, FLUX)
- Fine-tune prompts
- Add post-processing

---

## 12. TESTING STRATEGY

### Unit Tests

```csharp
[Test]
public async Task GeminiTrendDetector_ReturnsValidTrends()
{
    var detector = new GeminiTrendDetector(_geminiService);
    var trends = await detector.DetectTrendsAsync("Vietnam");

    Assert.IsNotEmpty(trends);
    Assert.IsTrue(trends.All(t => !string.IsNullOrEmpty(t.Topic)));
    Assert.IsTrue(trends.All(t => t.Score > 0));
}

[Test]
public async Task GeminiContentGenerator_GeneratesForAllPlatforms()
{
    var generator = new GeminiContentGenerator(_geminiService, _brandVoice);
    var content = await generator.GenerateMultiPlatformContentAsync(analysis, trend);

    Assert.AreEqual(6, content.Count); // 6 platforms
    Assert.IsTrue(content.ContainsKey("facebook"));
    Assert.IsTrue(content.ContainsKey("tiktok"));
}
```

### Integration Tests

```csharp
[Test]
[Category("Integration")]
public async Task EndToEnd_TrendToPost()
{
    // 1. Detect trends
    var trends = await _trendDetector.DetectTrendsAsync();
    var hotTrend = trends.First();

    // 2. Analyze viral content
    var analysis = await _videoAnalyzer.AnalyzeViralVideoAsync(hotTrend.VideoUrl);

    // 3. Generate content
    var content = await _contentGenerator.GenerateForPlatformAsync(analysis, hotTrend, "facebook");

    // 4. Generate image
    var imageUrl = await _imageGenerator.GenerateImageAsync(content.ImagePrompt);

    Assert.IsNotNull(imageUrl);
    Assert.IsTrue(imageUrl.StartsWith("http"));
}
```

---

## 13. MONITORING & OPTIMIZATION

### Track These Metrics

```csharp
public class AIMetrics
{
    public int GeminiCalls { get; set; }
    public int BananaCalls { get; set; }
    public int VeoCalls { get; set; }

    public decimal TotalCost { get; set; }
    public TimeSpan AverageResponseTime { get; set; }

    public int SuccessfulGenerations { get; set; }
    public int FailedGenerations { get; set; }

    public Dictionary<string, int> CallsByModel { get; set; }
}
```

### Optimization Tips

1. **Cache Results**
   - Cache trend analysis for 1 hour
   - Cache video transcriptions
   - Don't regenerate same content

2. **Batch Requests**
   - Generate content for all platforms in one prompt
   - Process multiple comments together

3. **Use Right Model**
   - Flash for simple tasks (90% of cases)
   - Pro only for complex analysis

4. **Monitor Costs**
   - Set spending alerts
   - Log all API calls
   - Optimize prompts

---

## 14. MIGRATION PLAN (If Needed Later)

**If we need to add other AIs:**

```csharp
public interface IAIService
{
    Task<string> GenerateContentAsync(string prompt);
    Task<VideoAnalysis> AnalyzeVideoAsync(string videoUrl);
    Task<CommentReply> GenerateReplyAsync(Comment comment);
}

// Gemini implementation
public class GeminiAIService : IAIService { ... }

// Future: Claude implementation
public class ClaudeAIService : IAIService { ... }

// Factory pattern
public class AIServiceFactory
{
    public IAIService Create(AIProvider provider)
    {
        return provider switch
        {
            AIProvider.Gemini => new GeminiAIService(),
            AIProvider.Claude => new ClaudeAIService(),
            _ => new GeminiAIService() // Default
        };
    }
}
```

**Easy to migrate if needed, but start with Gemini!**

---

## 15. CONCLUSION

### Summary

**Gemini-First Strategy:**
- ✅ Simple architecture
- ✅ 47% cost savings
- ✅ Faster development
- ✅ Easier maintenance
- ✅ Better integration
- ✅ Google ecosystem benefits

**Implementation Stack:**
```
Primary AI: Google Gemini (Flash + Pro)
Images: Banana API
Videos: Veo 3
Total Services: 3 (vs 8+ before)
```

**Development Time:**
- 8 weeks (vs 12 weeks multi-AI)
- Simpler codebase
- Easier to onboard team

**Costs:**
- $450/month (vs $850/month)
- Predictable pricing
- Free tier for testing

### Decision: ✅ GO WITH GEMINI-FIRST

**Start simple, scale if needed!**

---

**Next Steps:**

1. ✅ Get Google AI API key
2. ✅ Get Banana API key
3. ✅ Setup development environment
4. ✅ Implement GeminiAIService
5. ✅ Test with trend detection
6. ✅ Build from there!

🚀 **Ready to code with Gemini!**
