# Engagement Automation - AI Comment Reply, Auto Like, Auto Comment

## Tổng Quan

Engagement automation là **CHÌA KHÓA** để nurture fanpage hiệu quả. Sau khi đăng bài, việc tương tác với audience quyết định:
- Algorithm boost (platforms ưu tiên posts có nhiều engagement)
- Community building
- Lead generation
- Brand loyalty

Tài liệu này bao gồm:
1. **AI Comment Reply** - Tự động trả lời comments bằng AI
2. **Auto Comment** - Tự động comment vào posts khác
3. **Auto Like** - Tự động like comments/posts
4. **Sentiment Analysis** - Phân tích cảm xúc để respond phù hợp
5. **Lead Capture** - Chuyển đổi comments thành leads
6. **Compliance & Safety** - Tránh bị ban

---

## 1. TẠI SAO CẦN ENGAGEMENT AUTOMATION?

### 1.1 Lợi Ích

#### Algorithm Boost 📈
```
Nhiều comments/likes → Platform algorithm thấy "engaging"
→ Push content tới nhiều người hơn → Organic reach tăng
```

**Data**: Posts với >20 comments trong 1 giờ đầu có reach cao hơn 300%!

#### Time Savings ⏰
```
Manual: 50 comments/hour = 1.2 phút/comment
AI Auto: 500 comments/hour = 100ms/comment

Tiết kiệm: 90% thời gian
```

#### 24/7 Availability 🌍
- User comment lúc 2 AM? → AI reply ngay
- Không bỏ lỡ bất kỳ engagement nào
- Users ở time zones khác nhau đều được phục vụ

#### Consistent Brand Voice 🎯
- AI trained với brand guidelines
- Tone consistent across all replies
- Không có "bad day" replies

#### Lead Generation 🎁
```
User comment: "Giá bao nhiêu?"
→ AI reply: "Price là X. Inbox để nhận discount code!"
→ User inbox
→ Chatbot qualify lead
→ Sales team follow up
```

---

### 1.2 Khi Nào KHÔNG Nên Automation?

❌ **Complaints & Negative Comments**
- Cần human touch
- Empathy và understanding
- Có thể escalate nhanh

❌ **Complex Questions**
- Technical support
- Custom requests
- Refund/return issues

❌ **Crisis Management**
- PR issues
- Major bugs/outages
- Controversy

❌ **VIP/High-Value Customers**
- Personal touch important
- Relationship building
- Custom deals

**Solution**: AI detect và escalate tới human!

---

## 2. AI COMMENT REPLY

### 2.1 How It Works

```
[User comments on your post]
         ↓
[Webhook/Polling detects new comment]
         ↓
[AI analyzes comment (sentiment, intent, keywords)]
         ↓
[Generate personalized reply using Claude/GPT-4]
         ↓
[Quality check (spam filter, policy compliance)]
         ↓
[Post reply via API]
         ↓
[Track engagement & learn]
```

### 2.2 Implementation

#### Step 1: Comment Monitoring

```csharp
public class CommentMonitor
{
    private readonly FacebookClient _facebook;
    private readonly InstagramClient _instagram;
    private readonly TikTokClient _tiktok;
    private readonly TwitterClient _twitter;
    private readonly IRepository<PostedContent> _postRepo;

    // Option 1: Webhook (Real-time, Recommended)
    public async Task SetupWebhooksAsync()
    {
        // Facebook/Instagram Webhooks
        await _facebook.SubscribeToPageAsync(
            events: new[] { "feed", "comments" },
            callbackUrl: "https://yourdomain.com/webhooks/facebook"
        );

        // TikTok Webhooks
        await _tiktok.SubscribeToEventsAsync(
            events: new[] { "video.comment" },
            callbackUrl: "https://yourdomain.com/webhooks/tiktok"
        );
    }

    // Webhook Handler
    [HttpPost("/webhooks/facebook")]
    public async Task<IActionResult> HandleFacebookWebhook([FromBody] FacebookWebhookPayload payload)
    {
        foreach (var entry in payload.Entry)
        {
            foreach (var change in entry.Changes)
            {
                if (change.Field == "feed" && change.Value.Item == "comment")
                {
                    var comment = change.Value.Comment;
                    await ProcessCommentAsync(comment, "facebook");
                }
            }
        }

        return Ok();
    }

    // Option 2: Polling (Backup if webhooks not available)
    public async Task PollForNewCommentsAsync()
    {
        var recentPosts = await _postRepo.FindAsync(p =>
            p.PostedAt > DateTime.UtcNow.AddHours(-24) &&
            p.Status == ContentStatus.Posted
        );

        foreach (var post in recentPosts)
        {
            var comments = await GetCommentsAsync(post.Platform, post.PlatformPostId);

            foreach (var comment in comments)
            {
                // Check if already processed
                if (!await HasProcessedCommentAsync(comment.Id))
                {
                    await ProcessCommentAsync(comment, post.Platform);
                }
            }
        }
    }
}
```

#### Step 2: Comment Analysis

```csharp
public class CommentAnalyzer
{
    private readonly ClaudeClient _claude;
    private readonly SentimentAnalyzer _sentiment;

    public async Task<CommentAnalysis> AnalyzeAsync(Comment comment)
    {
        // 1. Sentiment Analysis
        var sentiment = await _sentiment.AnalyzeAsync(comment.Text);

        // 2. Intent Classification
        var intent = await ClassifyIntentAsync(comment.Text);

        // 3. Keyword Extraction
        var keywords = await ExtractKeywordsAsync(comment.Text);

        // 4. Spam Detection
        var isSpam = await DetectSpamAsync(comment);

        // 5. Language Detection
        var language = DetectLanguage(comment.Text);

        return new CommentAnalysis
        {
            Sentiment = sentiment, // Positive/Negative/Neutral/Mixed
            Intent = intent,       // Question/Compliment/Complaint/Inquiry/Other
            Keywords = keywords,
            IsSpam = isSpam,
            Language = language,
            RequiresHumanReview = DetermineIfNeedsHuman(sentiment, intent, keywords)
        };
    }

    private async Task<CommentIntent> ClassifyIntentAsync(string text)
    {
        var prompt = $@"
        Classify the intent of this comment:
        ""{text}""

        Categories:
        - QUESTION: Asking for information
        - INQUIRY: Interested in product/service (""How much?"", ""Where to buy?"")
        - COMPLIMENT: Positive feedback
        - COMPLAINT: Negative feedback, issue report
        - SPAM: Irrelevant, promotional
        - OTHER: General engagement

        Return only the category name.";

        var response = await _claude.ChatAsync(prompt);
        return Enum.Parse<CommentIntent>(response.Trim());
    }

    private bool DetermineIfNeedsHuman(Sentiment sentiment, CommentIntent intent, List<string> keywords)
    {
        // Escalate to human if:
        if (sentiment == Sentiment.Negative && intent == CommentIntent.COMPLAINT)
            return true;

        if (keywords.Any(k => k.Contains("refund", StringComparison.OrdinalIgnoreCase) ||
                              k.Contains("scam", StringComparison.OrdinalIgnoreCase) ||
                              k.Contains("lawyer", StringComparison.OrdinalIgnoreCase)))
            return true;

        if (intent == CommentIntent.SPAM)
            return false; // Don't reply to spam

        return false; // AI can handle
    }
}

public enum CommentIntent
{
    QUESTION,
    INQUIRY,
    COMPLIMENT,
    COMPLAINT,
    SPAM,
    OTHER
}
```

#### Step 3: AI Reply Generation

```csharp
public class AIReplyGenerator
{
    private readonly ClaudeClient _claude;
    private readonly BrandVoiceConfig _brandVoice;

    public async Task<string> GenerateReplyAsync(
        Comment comment,
        CommentAnalysis analysis,
        PostContext postContext)
    {
        var prompt = BuildPrompt(comment, analysis, postContext);
        var reply = await _claude.ChatAsync(prompt);

        // Post-process
        reply = EnsureBrandCompliance(reply);
        reply = AddCallToActionIfNeeded(reply, analysis.Intent);

        return reply;
    }

    private string BuildPrompt(Comment comment, CommentAnalysis analysis, PostContext postContext)
    {
        var prompt = $@"
You are a social media manager for {_brandVoice.BrandName}.

Brand Voice Guidelines:
- Tone: {_brandVoice.Tone} (e.g., Friendly, Professional, Humorous)
- Language: {_brandVoice.Language}
- Style: {_brandVoice.Style}
- DO: {string.Join(", ", _brandVoice.Dos)}
- DON'T: {string.Join(", ", _brandVoice.Donts)}

Original Post Context:
- Topic: {postContext.Topic}
- Content: {postContext.Content}

User Comment:
""{comment.Text}""

Comment Analysis:
- Sentiment: {analysis.Sentiment}
- Intent: {analysis.Intent}
- Keywords: {string.Join(", ", analysis.Keywords)}

Task: Generate a natural, engaging reply that:
1. Addresses the user's {analysis.Intent}
2. Matches our brand voice
3. Is concise (< 100 words)
4. Encourages further engagement
5. {GetIntentSpecificInstructions(analysis.Intent)}

IMPORTANT:
- Write in {analysis.Language}
- Use emojis sparingly (max 1-2)
- Be authentic, not robotic
- If it's a question, answer it directly

Reply:";

        return prompt;
    }

    private string GetIntentSpecificInstructions(CommentIntent intent)
    {
        return intent switch
        {
            CommentIntent.QUESTION => "Answer the question directly and accurately",
            CommentIntent.INQUIRY => "Provide helpful info and suggest next steps (DM, visit website, etc.)",
            CommentIntent.COMPLIMENT => "Thank them genuinely and invite them to share with friends",
            CommentIntent.COMPLAINT => "Acknowledge concern empathetically and offer to help via DM",
            CommentIntent.OTHER => "Engage naturally and keep conversation going",
            _ => "Respond appropriately"
        };
    }

    private string AddCallToActionIfNeeded(string reply, CommentIntent intent)
    {
        if (intent == CommentIntent.INQUIRY)
        {
            reply += "\n\nInbox chúng mình để nhận thêm thông tin chi tiết nhé! 💬";
        }
        else if (intent == CommentIntent.COMPLIMENT)
        {
            reply += " Tag bạn bè để cùng trải nghiệm nhé! 🎉";
        }

        return reply;
    }
}
```

#### Step 4: Reply Posting with Safety Checks

```csharp
public class CommentReplyService
{
    private readonly AIReplyGenerator _replyGenerator;
    private readonly CommentAnalyzer _analyzer;
    private readonly Dictionary<string, ISocialMediaClient> _clients;
    private readonly ILogger<CommentReplyService> _logger;

    public async Task ProcessCommentAsync(Comment comment, string platform)
    {
        try
        {
            // 1. Analyze comment
            var analysis = await _analyzer.AnalyzeAsync(comment);

            // 2. Check if should reply
            if (!ShouldReply(analysis))
            {
                _logger.LogInformation($"Skipping comment {comment.Id}: {GetSkipReason(analysis)}");
                return;
            }

            // 3. Check if needs human
            if (analysis.RequiresHumanReview)
            {
                await NotifyHumanAsync(comment, analysis);
                return;
            }

            // 4. Generate reply
            var postContext = await GetPostContextAsync(comment.PostId);
            var reply = await _replyGenerator.GenerateReplyAsync(comment, analysis, postContext);

            // 5. Safety checks
            if (!await PassesSafetyChecksAsync(reply))
            {
                _logger.LogWarning($"Reply failed safety check for comment {comment.Id}");
                await NotifyHumanAsync(comment, analysis, reply);
                return;
            }

            // 6. Post reply
            var client = _clients[platform];
            await client.ReplyToCommentAsync(comment.Id, reply);

            // 7. Save to database
            await SaveReplyRecordAsync(comment, reply, analysis);

            // 8. Track metrics
            await TrackReplyMetricsAsync(comment.Id, reply);

            _logger.LogInformation($"Successfully replied to comment {comment.Id} on {platform}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing comment {comment.Id}");
        }
    }

    private bool ShouldReply(CommentAnalysis analysis)
    {
        // Don't reply to spam
        if (analysis.IsSpam) return false;

        // Don't reply to our own comments
        if (analysis.IsOwnComment) return false;

        // Check rate limit (don't spam)
        if (analysis.UserHasRecentlyCommented && analysis.RecentCommentCount > 3)
            return false; // User is spamming

        return true;
    }

    private async Task<bool> PassesSafetyChecksAsync(string reply)
    {
        // Check 1: No profanity
        if (ContainsProfanity(reply))
            return false;

        // Check 2: No spam patterns
        if (LooksLikeSpam(reply))
            return false;

        // Check 3: Not too long
        if (reply.Length > 500)
            return false;

        // Check 4: Platform policy compliance
        if (!await IsCompliantAsync(reply))
            return false;

        return true;
    }
}
```

---

### 2.3 Advanced Features

#### Context-Aware Replies

```csharp
public class ContextAwareReplyGenerator : AIReplyGenerator
{
    // Remember conversation history
    public async Task<string> GenerateReplyAsync(
        Comment comment,
        List<Comment> conversationHistory)
    {
        var prompt = $@"
Previous conversation:
{string.Join("\n", conversationHistory.Select(c => $"{c.Author}: {c.Text}"))}

Latest comment:
{comment.Author}: {comment.Text}

Generate contextual reply considering the conversation flow.
        ";

        return await _claude.ChatAsync(prompt);
    }
}
```

#### Multi-Language Support

```csharp
public async Task<string> GenerateMultilingualReplyAsync(Comment comment)
{
    var detectedLanguage = DetectLanguage(comment.Text);

    var prompt = $@"
Respond to this comment in {detectedLanguage}:
""{comment.Text}""

Use natural, native {detectedLanguage} expressions.
    ";

    return await _claude.ChatAsync(prompt);
}
```

#### Personalized Replies

```csharp
public async Task<string> GeneratePersonalizedReplyAsync(Comment comment)
{
    // Get user history
    var userHistory = await GetUserHistoryAsync(comment.AuthorId);

    var prompt = $@"
User profile:
- Name: {userHistory.Name}
- Previous interactions: {userHistory.InteractionCount}
- Customer status: {(userHistory.HasPurchased ? "Existing customer" : "Potential customer")}
- Interests: {string.Join(", ", userHistory.Interests)}

Comment: ""{comment.Text}""

Generate personalized reply acknowledging their history with us.
    ";

    return await _claude.ChatAsync(prompt);
}
```

---

## 3. AUTO COMMENT

### 3.1 Use Cases

#### Engagement Boost
- Comment vào posts của influencers trong niche
- First comment để tăng visibility
- Join trending conversations

#### Community Building
- Comment vào posts của followers
- Acknowledge user-generated content
- Build relationships

#### Cross-Promotion
- Comment vào posts liên quan
- Soft promotion (not spammy!)

### 3.2 Implementation

```csharp
public class AutoCommentService
{
    private readonly AIReplyGenerator _replyGenerator;
    private readonly IRepository<TargetAccount> _targetAccounts;

    public async Task RunAutoCommentCampaignAsync()
    {
        // 1. Get target accounts (influencers, competitors, related brands)
        var targets = await _targetAccounts.FindAsync(t => t.IsActive);

        foreach (var target in targets)
        {
            // 2. Get recent posts
            var posts = await GetRecentPostsAsync(target.Platform, target.Username);

            // 3. Filter posts worth commenting on
            var worthyPosts = posts
                .Where(p => IsWorthCommenting(p))
                .OrderByDescending(p => p.EngagementRate)
                .Take(3) // Max 3 posts per account per day
                .ToList();

            foreach (var post in worthyPosts)
            {
                // 4. Generate contextual comment
                var comment = await GenerateContextualCommentAsync(post, target);

                // 5. Safety check
                if (await PassesSafetyChecksAsync(comment))
                {
                    // 6. Post comment
                    await PostCommentAsync(post.Platform, post.Id, comment);

                    // 7. Wait to avoid rate limit
                    await Task.Delay(TimeSpan.FromMinutes(5));
                }
            }
        }
    }

    private bool IsWorthCommenting(Post post)
    {
        // High engagement posts
        if (post.LikeCount > 1000 || post.CommentCount > 100)
            return true;

        // Recent and trending
        if (post.CreatedAt > DateTime.UtcNow.AddHours(-6) &&
            post.EngagementRate > 0.05)
            return true;

        // Relevant to our niche
        if (post.Topics.Any(t => _brandVoice.RelevantTopics.Contains(t)))
            return true;

        return false;
    }

    private async Task<string> GenerateContextualCommentAsync(Post post, TargetAccount target)
    {
        var prompt = $@"
Generate a natural, engaging comment for this post:

Author: {post.Author}
Content: {post.Content}
Topic: {string.Join(", ", post.Topics)}

Guidelines:
1. Be genuine and authentic (NOT promotional!)
2. Add value to the conversation
3. Show you actually read the post
4. Keep it brief (< 50 words)
5. Use 1 emoji max
6. NO self-promotion
7. NO links

Goal: Build genuine relationship and visibility.

Comment:";

        var comment = await _claude.ChatAsync(prompt);

        return comment;
    }
}
```

### 3.3 Safety & Compliance

⚠️ **CRITICAL**: Auto-commenting có rủi ro CAO!

#### DO ✅:
```csharp
var rules = new AutoCommentRules
{
    MaxCommentsPerDay = 20,        // Don't spam
    MaxCommentsPerAccount = 3,     // Per target account
    MinIntervalMinutes = 5,        // Space out comments
    OnlyOnRelevantPosts = true,    // Stay in niche
    NoDirectPromotion = true,      // Be authentic
    RequireHumanApproval = true    // Review before posting (recommended)
};
```

#### DON'T ❌:
- ❌ Spam same comment everywhere
- ❌ Promote your products/services directly
- ❌ Comment on every post from same account
- ❌ Use generic comments ("Nice!", "Great post!")
- ❌ Comment too fast (looks like bot)

#### Example Comments

**❌ BAD - Spammy**:
```
"Great post! 😍
Check out our products at [link]!"
```

**❌ BAD - Generic**:
```
"Nice! 👍"
```

**✅ GOOD - Valuable**:
```
"Love this perspective on [topic]! We've been seeing similar
trends with [relevant insight]. Have you tried [relevant question]?"
```

**✅ GOOD - Authentic Engagement**:
```
"This is so helpful! The tip about [specific detail] is
exactly what I needed. Thanks for sharing!"
```

---

## 4. AUTO LIKE

### 4.1 Why Auto Like?

- **Algorithm boost**: More likes = more reach
- **Community building**: Show support to followers
- **Relationship building**: Engage with influencers
- **Reciprocity**: They might like your posts back

### 4.2 Implementation

```csharp
public class AutoLikeService
{
    private readonly Dictionary<string, ISocialMediaClient> _clients;
    private readonly IRepository<TargetAccount> _targets;
    private readonly IRepository<LikeHistory> _likeHistory;

    public async Task RunAutoLikeCampaignAsync()
    {
        // 1. Get posts to like from different sources
        var postsToLike = new List<Post>();

        // Source 1: Followers' posts
        postsToLike.AddRange(await GetFollowersPostsAsync());

        // Source 2: Target influencers
        postsToLike.AddRange(await GetTargetInfluencerPostsAsync());

        // Source 3: Posts mentioning your brand
        postsToLike.AddRange(await GetBrandMentionsAsync());

        // Source 4: Posts using your hashtags
        postsToLike.AddRange(await GetHashtagPostsAsync(_brandVoice.BrandHashtags));

        // 2. Filter
        postsToLike = postsToLike
            .Where(p => !await HasLikedAsync(p.Id))
            .Where(p => IsAppropriateToLike(p))
            .OrderByDescending(p => p.Priority)
            .Take(100) // Daily limit
            .ToList();

        // 3. Like posts
        foreach (var post in postsToLike)
        {
            try
            {
                var client = _clients[post.Platform];
                await client.LikePostAsync(post.Id);

                // Save history
                await _likeHistory.AddAsync(new LikeHistory
                {
                    PostId = post.Id,
                    Platform = post.Platform,
                    LikedAt = DateTime.UtcNow
                });

                // Rate limiting
                await Task.Delay(Random.Shared.Next(3000, 8000)); // 3-8 seconds
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to like post {post.Id}");
            }
        }
    }

    private bool IsAppropriateToLike(Post post)
    {
        // Don't like controversial content
        if (post.Topics.Any(t => _blacklistedTopics.Contains(t)))
            return false;

        // Don't like spam
        if (post.IsSpam)
            return false;

        // Don't like posts with negative sentiment toward your brand
        if (post.MentionsBrand && post.Sentiment == Sentiment.Negative)
            return false;

        return true;
    }

    // Auto-like comments on YOUR posts
    public async Task AutoLikeOwnPostCommentsAsync()
    {
        var recentPosts = await GetRecentOwnPostsAsync();

        foreach (var post in recentPosts)
        {
            var comments = await GetCommentsAsync(post.Id);

            foreach (var comment in comments)
            {
                if (!comment.IsFromBrandAccount && !await HasLikedCommentAsync(comment.Id))
                {
                    await LikeCommentAsync(comment.Id);
                    await Task.Delay(2000); // 2 second delay
                }
            }
        }
    }
}
```

### 4.3 Strategy: Smart Auto-Like

#### Priority System

```csharp
public class LikePriorityCalculator
{
    public int CalculatePriority(Post post, User author)
    {
        int priority = 0;

        // High priority: Brand mentions
        if (post.MentionsBrand)
            priority += 50;

        // High priority: Existing customers
        if (author.IsCustomer)
            priority += 40;

        // Medium priority: Active followers
        if (author.IsFollower && author.EngagementRate > 0.05)
            priority += 30;

        // Medium priority: Target influencers
        if (_targetInfluencers.Contains(author.Id))
            priority += 35;

        // Low priority: Recent posts (more likely to be seen)
        if (post.CreatedAt > DateTime.UtcNow.AddHours(-2))
            priority += 10;

        // Bonus: High engagement posts (algorithm boost)
        if (post.EngagementRate > 0.1)
            priority += 20;

        return priority;
    }
}
```

#### Time-Based Liking

```csharp
public async Task SmartTimedLikingAsync()
{
    // Like posts at optimal times for maximum visibility
    var optimalTimes = new[]
    {
        new TimeSpan(9, 0, 0),   // Morning
        new TimeSpan(12, 30, 0), // Lunch
        new TimeSpan(19, 0, 0)   // Evening
    };

    var now = DateTime.Now.TimeOfDay;
    var nextOptimalTime = optimalTimes
        .Where(t => t > now)
        .OrderBy(t => t)
        .FirstOrDefault();

    if (nextOptimalTime == default)
    {
        // Schedule for tomorrow morning
        nextOptimalTime = optimalTimes[0];
    }

    var delay = nextOptimalTime - now;
    await Task.Delay(delay);

    // Run liking campaign
    await RunAutoLikeCampaignAsync();
}
```

### 4.4 Rate Limits & Safety

```csharp
public class AutoLikeRateLimiter
{
    private readonly Dictionary<string, PlatformLimits> _limits = new()
    {
        ["facebook"] = new PlatformLimits
        {
            MaxLikesPerHour = 50,
            MaxLikesPerDay = 500,
            MinDelaySeconds = 3,
            MaxDelaySeconds = 10
        },
        ["instagram"] = new PlatformLimits
        {
            MaxLikesPerHour = 60,
            MaxLikesPerDay = 1000,
            MinDelaySeconds = 2,
            MaxDelaySeconds = 8
        },
        ["tiktok"] = new PlatformLimits
        {
            MaxLikesPerHour = 100,
            MaxLikesPerDay = 500,
            MinDelaySeconds = 1,
            MaxDelaySeconds = 5
        },
        ["twitter"] = new PlatformLimits
        {
            MaxLikesPerHour = 1000,  // Twitter is more lenient
            MaxLikesPerDay = 1000,
            MinDelaySeconds = 0,
            MaxDelaySeconds = 2
        }
    };

    public async Task<bool> CanLikeAsync(string platform)
    {
        var limits = _limits[platform];
        var hourlyCount = await GetLikeCountAsync(platform, hours: 1);
        var dailyCount = await GetLikeCountAsync(platform, hours: 24);

        return hourlyCount < limits.MaxLikesPerHour &&
               dailyCount < limits.MaxLikesPerDay;
    }

    public async Task WaitBeforeNextLikeAsync(string platform)
    {
        var limits = _limits[platform];
        var delayMs = Random.Shared.Next(
            limits.MinDelaySeconds * 1000,
            limits.MaxDelaySeconds * 1000
        );

        await Task.Delay(delayMs);
    }
}

public class PlatformLimits
{
    public int MaxLikesPerHour { get; set; }
    public int MaxLikesPerDay { get; set; }
    public int MinDelaySeconds { get; set; }
    public int MaxDelaySeconds { get; set; }
}
```

---

## 5. SENTIMENT ANALYSIS & SMART ROUTING

### 5.1 Advanced Sentiment Detection

```csharp
public class AdvancedSentimentAnalyzer
{
    private readonly ClaudeClient _claude;

    public async Task<DetailedSentiment> AnalyzeAsync(string text)
    {
        var prompt = $@"
Analyze the sentiment and emotions in this comment:
""{text}""

Provide:
1. Overall sentiment (Positive/Negative/Neutral/Mixed)
2. Emotion (Joy, Anger, Sadness, Fear, Surprise, Disgust, None)
3. Intensity (Low/Medium/High)
4. Urgency (Low/Medium/High)
5. Intent (Question, Complaint, Compliment, Inquiry, Spam)
6. Requires human response? (Yes/No with reason)

JSON format:
{{
    ""sentiment"": ""..."",
    ""emotion"": ""..."",
    ""intensity"": ""..."",
    ""urgency"": ""..."",
    ""intent"": ""..."",
    ""requiresHuman"": {{
        ""value"": true/false,
        ""reason"": ""...""
    }}
}}";

        var response = await _claude.ChatAsync(prompt);
        return JsonSerializer.Deserialize<DetailedSentiment>(response);
    }
}

public class DetailedSentiment
{
    public string Sentiment { get; set; }
    public string Emotion { get; set; }
    public string Intensity { get; set; }
    public string Urgency { get; set; }
    public string Intent { get; set; }
    public RequiresHumanDecision RequiresHuman { get; set; }
}

public class RequiresHumanDecision
{
    public bool Value { get; set; }
    public string Reason { get; set; }
}
```

### 5.2 Smart Routing System

```csharp
public class CommentRoutingService
{
    public async Task RouteCommentAsync(Comment comment, DetailedSentiment sentiment)
    {
        if (sentiment.RequiresHuman.Value)
        {
            await RouteToHumanAsync(comment, sentiment);
            return;
        }

        if (sentiment.Urgency == "High")
        {
            await PrioritizeForAIAsync(comment, priority: Priority.High);
        }
        else if (sentiment.Intent == "Inquiry")
        {
            await RouteToLeadCaptureAsync(comment);
        }
        else
        {
            await RouteToStandardAIAsync(comment);
        }
    }

    private async Task RouteToHumanAsync(Comment comment, DetailedSentiment sentiment)
    {
        // Create ticket in support system
        var ticket = new SupportTicket
        {
            Source = "Social Media Comment",
            Platform = comment.Platform,
            CommentId = comment.Id,
            UserName = comment.Author,
            Content = comment.Text,
            Sentiment = sentiment,
            Priority = DeterminePriority(sentiment),
            CreatedAt = DateTime.UtcNow
        };

        await _ticketingSystem.CreateTicketAsync(ticket);

        // Notify team via Slack/Email
        await _notificationService.NotifyTeamAsync(
            channel: "support",
            message: $"🚨 New high-priority comment requires attention!\n" +
                     $"Platform: {comment.Platform}\n" +
                     $"Reason: {sentiment.RequiresHuman.Reason}\n" +
                     $"Link: {comment.Url}"
        );

        // Auto-acknowledge to user
        await ReplyWithHoldingMessageAsync(comment);
    }

    private async Task ReplyWithHoldingMessageAsync(Comment comment)
    {
        var message = comment.Platform switch
        {
            "facebook" => "Cảm ơn bạn đã liên hệ! Chúng mình sẽ phản hồi trong vòng 2 giờ. 🙏",
            "instagram" => "Thanks for reaching out! We'll get back to you shortly. 💬",
            _ => "Thank you! We'll respond soon."
        };

        await _socialMediaClient.ReplyToCommentAsync(comment.Id, message);
    }

    private Priority DeterminePriority(DetailedSentiment sentiment)
    {
        if (sentiment.Emotion == "Anger" && sentiment.Intensity == "High")
            return Priority.Urgent;

        if (sentiment.Urgency == "High")
            return Priority.High;

        if (sentiment.Intent == "Complaint")
            return Priority.Medium;

        return Priority.Normal;
    }
}
```

---

## 6. LEAD CAPTURE FROM COMMENTS

### 6.1 Identify High-Intent Comments

```csharp
public class LeadCaptureService
{
    public async Task ProcessPotentialLeadAsync(Comment comment)
    {
        // Detect buying intent
        var hasIntent = await DetectBuyingIntentAsync(comment.Text);

        if (!hasIntent)
            return;

        // Reply with lead capture
        var reply = await GenerateLeadCaptureReplyAsync(comment);
        await _socialMediaClient.ReplyToCommentAsync(comment.Id, reply);

        // Create lead record
        await CreateLeadRecordAsync(comment);
    }

    private async Task<bool> DetectBuyingIntentAsync(string text)
    {
        var buyingSignals = new[]
        {
            "giá", "price", "bao nhiêu", "how much", "cost",
            "mua", "buy", "order", "đặt hàng",
            "ship", "giao hàng", "delivery",
            "còn hàng", "available", "in stock",
            "discount", "giảm giá", "promo", "khuyến mãi"
        };

        return buyingSignals.Any(signal =>
            text.Contains(signal, StringComparison.OrdinalIgnoreCase));
    }

    private async Task<string> GenerateLeadCaptureReplyAsync(Comment comment)
    {
        // Generate personalized reply với CTA
        return $@"Dạ bạn ơi! Để được tư vấn chi tiết và nhận ưu đãi đặc biệt,
bạn inbox cho shop nhé! 💬
Shop sẽ hỗ trợ ngay! ✨";
    }

    private async Task CreateLeadRecordAsync(Comment comment)
    {
        var lead = new Lead
        {
            Source = $"Comment on {comment.Platform}",
            Name = comment.Author,
            UserId = comment.AuthorId,
            Platform = comment.Platform,
            InitialMessage = comment.Text,
            Status = LeadStatus.New,
            Priority = LeadPriority.Hot,
            CreatedAt = DateTime.UtcNow
        };

        await _leadRepository.AddAsync(lead);

        // Trigger follow-up workflow
        await _leadWorkflow.StartAsync(lead.Id);
    }
}
```

### 6.2 Automated Lead Nurturing

```csharp
public class LeadNurturingWorkflow
{
    public async Task StartAsync(Guid leadId)
    {
        var lead = await _leadRepository.GetByIdAsync(leadId);

        // Step 1: Wait for DM (24 hours)
        await WaitForDirectMessageAsync(lead, hours: 24);

        if (lead.HasDirectMessaged)
        {
            // Hand off to chatbot or human
            await HandOffToChatbotAsync(lead);
        }
        else
        {
            // Step 2: Follow-up comment
            await PostFollowUpCommentAsync(lead);

            // Step 3: Wait again (48 hours)
            await WaitForDirectMessageAsync(lead, hours: 48);

            if (!lead.HasDirectMessaged)
            {
                // Mark as cold lead
                lead.Status = LeadStatus.Cold;
                await _leadRepository.UpdateAsync(lead);
            }
        }
    }

    private async Task PostFollowUpCommentAsync(Lead lead)
    {
        var followUp = "Bạn có muốn tìm hiểu thêm không ạ? Inbox shop để nhận tư vấn nhé! 😊";
        await _socialMediaClient.ReplyToCommentAsync(lead.InitialCommentId, followUp);
    }
}
```

---

## 7. COMPLIANCE & BEST PRACTICES

### 7.1 Platform Policies

#### Facebook/Instagram
✅ **Allowed**:
- Automated replies to comments/messages (via official API)
- Auto-moderation (spam, hate speech)
- Chatbots với proper permissions

❌ **Prohibited**:
- Fake engagement (buying likes/comments)
- Automation on personal profiles (only pages)
- Misleading bots

#### TikTok
✅ **Allowed**:
- Official API automation
- Comment moderation

❌ **Prohibited**:
- Fake engagement
- Spam comments
- Bots that impersonate humans

#### Twitter/X
✅ **Allowed**:
- Bots that clearly identify as bots
- Useful automated content

❌ **Prohibited**:
- Spam automation
- Aggressive following/unfollowing
- Reply spam

### 7.2 Best Practices Checklist

#### Comment Reply Automation
- [ ] Use official APIs only (no scraping)
- [ ] Disclose AI-powered responses when appropriate
- [ ] Escalate sensitive comments to humans
- [ ] Maintain consistent brand voice
- [ ] Include opt-out mechanism
- [ ] Monitor and audit AI responses
- [ ] Set rate limits to avoid spam flags
- [ ] A/B test response templates
- [ ] Track engagement metrics
- [ ] Regular quality reviews

#### Auto Comment/Like
- [ ] Target only relevant accounts
- [ ] Space out actions (rate limiting)
- [ ] Vary comment content (no copy-paste)
- [ ] Add genuine value (no spam)
- [ ] No self-promotion in comments
- [ ] Monitor for platform warnings
- [ ] Use business accounts only
- [ ] Set daily/hourly limits
- [ ] Implement human review workflow
- [ ] Regular compliance audits

### 7.3 Warning Signs & Troubleshooting

#### ⚠️ Platform Warning Signs
```
- "Your account is sending too many requests"
- "Unusual activity detected"
- Features temporarily restricted
- Comments hidden/deleted automatically
- Reach suddenly drops
```

**Action Plan**:
1. Immediately pause automation
2. Review recent activity logs
3. Check if rate limits exceeded
4. Review comment/like quality
5. Implement stricter limits
6. Wait 48-72 hours before resuming
7. Gradually ramp up (10% daily increase)

#### Common Issues

**Issue 1: Low Engagement on Auto-Comments**
```
Diagnosis: Generic, non-valuable comments
Solution:
- Improve AI prompts for more specific comments
- Add more context to prompts
- Human review comments before posting
```

**Issue 2: High "Requires Human" Rate**
```
Diagnosis: Too sensitive sentiment analysis
Solution:
- Tune sentiment thresholds
- Improve intent classification
- Build confidence in AI over time
```

**Issue 3: Replies Sound Robotic**
```
Diagnosis: Poor prompt engineering or brand voice
Solution:
- Refine brand voice guidelines
- Add more examples to prompts
- Use more conversational language in prompts
- A/B test different Claude/GPT-4 temperatures
```

---

## 8. MONITORING & ANALYTICS

### 8.1 Key Metrics to Track

```csharp
public class EngagementMetrics
{
    // Comment Reply Metrics
    public int TotalCommentsReceived { get; set; }
    public int CommentsRepliedByAI { get; set; }
    public int CommentsEscalatedToHuman { get; set; }
    public TimeSpan AverageResponseTime { get; set; }
    public double UserSatisfactionScore { get; set; } // Based on follow-up engagement

    // Auto Comment Metrics
    public int CommentsPosted { get; set; }
    public int CommentsWithReplies { get; set; }
    public int CommentsLikedByOP { get; set; }
    public double CommentEngagementRate { get; set; }

    // Auto Like Metrics
    public int PostsLiked { get; set; }
    public int CommentsLiked { get; set; }
    public int ReciprocatedLikes { get; set; } // They liked back
    public int NewFollowersFromLikes { get; set; }

    // Lead Capture Metrics
    public int LeadsGenerated { get; set; }
    public int LeadsConverted { get; set; }
    public double LeadConversionRate { get; set; }

    // Compliance Metrics
    public int WarningsReceived { get; set; }
    public int FeatureRestrictionsCount { get; set; }
    public double AutomationHealthScore { get; set; }
}
```

### 8.2 Dashboard

```csharp
public class EngagementDashboard
{
    public async Task<DashboardData> GetDashboardDataAsync(DateTime from, DateTime to)
    {
        return new DashboardData
        {
            Overview = await GetOverviewMetricsAsync(from, to),
            ByPlatform = await GetPlatformBreakdownAsync(from, to),
            TopPerformingReplies = await GetTopRepliesAsync(from, to),
            QualityScore = await CalculateQualityScoreAsync(from, to),
            Alerts = await GetActiveAlertsAsync()
        };
    }

    private async Task<double> CalculateQualityScoreAsync(DateTime from, DateTime to)
    {
        var metrics = await GetEngagementMetricsAsync(from, to);

        // Quality score algorithm
        var score = 0.0;

        // Component 1: Response coverage (30%)
        score += (metrics.CommentsRepliedByAI / (double)metrics.TotalCommentsReceived) * 30;

        // Component 2: User satisfaction (40%)
        score += metrics.UserSatisfactionScore * 40;

        // Component 3: Human escalation rate (20% - lower is better)
        var escalationRate = metrics.CommentsEscalatedToHuman / (double)metrics.TotalCommentsReceived;
        score += (1 - escalationRate) * 20;

        // Component 4: Compliance (10%)
        score += (metrics.WarningsReceived == 0 ? 10 : 0);

        return Math.Round(score, 2);
    }
}
```

---

## 9. IMPLEMENTATION ROADMAP

### Phase 1: Foundation (Week 1-2)
- [ ] Setup webhook/polling for comment monitoring
- [ ] Implement basic sentiment analysis
- [ ] Build AI reply generator dengan simple prompts
- [ ] Manual approval workflow
- [ ] Basic metrics tracking

### Phase 2: Automation (Week 3-4)
- [ ] Auto-reply để common questions
- [ ] Smart routing (human vs AI)
- [ ] Auto-like implementation
- [ ] Rate limiting và safety checks
- [ ] Quality monitoring dashboard

### Phase 3: Advanced Features (Week 5-6)
- [ ] Context-aware replies (conversation history)
- [ ] Lead capture workflow
- [ ] Multi-language support
- [ ] Auto-comment campaign management
- [ ] Advanced sentiment analysis

### Phase 4: Optimization (Week 7-8)
- [ ] A/B testing framework
- [ ] Machine learning cho better routing
- [ ] Personalization based on user history
- [ ] Cross-platform coordination
- [ ] Comprehensive analytics

---

## 10. COST ESTIMATION

### AI API Costs (Monthly)

**Scenario 1: Small (1000 comments/day)**
```
Comment Analysis: 1000 × $0.002 = $2/day = $60/month
Reply Generation: 800 × $0.01 = $8/day = $240/month
Total: ~$300/month
```

**Scenario 2: Medium (5000 comments/day)**
```
Comment Analysis: 5000 × $0.002 = $10/day = $300/month
Reply Generation: 4000 × $0.01 = $40/day = $1,200/month
Total: ~$1,500/month
```

**Scenario 3: Large (20000 comments/day)**
```
Comment Analysis: 20000 × $0.002 = $40/day = $1,200/month
Reply Generation: 15000 × $0.01 = $150/day = $4,500/month
Total: ~$5,700/month
```

### ROI Calculation

**Time Savings**:
```
Manual: 1 min/comment × 1000 comments = 16.7 hours/day
Cost: 16.7 hours × $20/hour = $334/day = $10,020/month

Automation Cost: $300/month

Savings: $9,720/month
```

**Engagement Boost**:
```
Faster responses → +30% engagement
More engagement → +50% algorithm reach
More reach → +25% leads

ROI: Massive! 🚀
```

---

## 11. CONCLUSION

### Key Takeaways

1. **Engagement Automation is CRITICAL** - 30-50% boost in reach/engagement
2. **AI Reply > Manual** - Faster, consistent, scalable
3. **Human-in-Loop Essential** - For sensitive/complex cases
4. **Compliance First** - Use official APIs, follow rules
5. **Quality > Quantity** - Better to reply well to 100 than poorly to 1000

### Success Metrics

**Good Automation**:
- Response time < 5 minutes
- User satisfaction > 80%
- Human escalation rate < 20%
- Zero platform warnings
- Positive sentiment growth

**Great Automation**:
- Response time < 1 minute
- User satisfaction > 90%
- Human escalation rate < 10%
- Lead conversion rate > 5%
- Community growth > 20%/month

### Next Steps

1. **Start Small**: Implement comment reply cho 1 platform first
2. **Test Thoroughly**: Manual review for first 2 weeks
3. **Iterate**: Improve prompts based on feedback
4. **Scale Gradually**: Add platforms and features incrementally
5. **Monitor Closely**: Watch metrics and compliance daily

---

**Development Time**: 6-8 weeks
**Monthly Cost**: $300-5,700 (depending on volume)
**Expected ROI**: 10-30x (time savings + engagement boost)

Ready to automate! 🚀
