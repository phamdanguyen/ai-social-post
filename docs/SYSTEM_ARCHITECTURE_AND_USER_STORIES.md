# System Architecture & User Stories (SIMPLIFIED)

## Tổng Quan

Tool đơn giản để:
1. Tìm trends tự động
2. AI viết lại content
3. Đăng bài lên 5 platforms
4. AI reply comments

**1 app WinForms, SQLite, chạy trên 1 máy.**

---

## PHẦN 1: USER PERSONAS

### Persona 1: Marketing Manager - Nga
- Quản lý 2-3 fanpages
- Cần post content mỗi ngày
- Muốn tăng engagement
- Không có thời gian reply hết comments

### Persona 2: Content Creator - Minh
- Có kênh YouTube và TikTok
- Tạo content hàng ngày
- Muốn theo kịp trends
- Cần cross-post nhanh

### Persona 3: Small Business Owner - Hùng
- Bán hàng qua Facebook/Zalo
- Post sản phẩm mỗi ngày
- Phải trả lời comments về giá, ship
- Một mình làm tất cả

---

## PHẦN 2: USER STORIES

### Epic 1: Trend Detection & Content Creation

**US-001: Tự Động Phát Hiện Trends**
```
AS A Marketing Manager
I WANT AI to detect trending topics automatically
SO THAT I can create relevant content quickly

Acceptance Criteria:
- AI checks trends every 15-30 minutes
- Shows trend score (0-100)
- Supports 5 platforms: Facebook, TikTok, YouTube, Zalo, X
- Filters by Vietnam region
```

**US-002: AI Tạo Content**
```
AS A Content Creator
I WANT AI to generate content from trends
SO THAT I can post quickly

Acceptance Criteria:
- Generates content for all 5 platforms
- Adapts tone per platform
- Suggests hashtags
- Provides caption and description
```

**US-003: Review và Edit Content**
```
AS A Marketing Manager
I WANT to review AI content before posting
SO THAT I can ensure quality

Acceptance Criteria:
- Shows preview for each platform
- Allows editing
- Can save as draft
- Can post immediately or schedule
```

---

### Epic 2: Smart Scheduling

**US-004: Đăng Bài Ngay**
```
AS A Content Creator
I WANT to post immediately
SO THAT I don't miss trending opportunities

Acceptance Criteria:
- "Post Now" button
- Posts to selected platforms
- Shows progress
- Error handling
```

**US-005: Lên Lịch Đăng Bài**
```
AS A Marketing Manager
I WANT to schedule posts
SO THAT content goes live at optimal times

Acceptance Criteria:
- Manual time selection
- AI suggests best time
- Calendar view
- Can cancel scheduled posts
```

---

### Epic 3: Engagement Automation

**US-006: AI Reply Comments**
```
AS A Small Business Owner
I WANT AI to reply to common questions
SO THAT customers get quick responses

Acceptance Criteria:
- Monitors comments in real-time
- Auto-replies to FAQs (price, shipping)
- Reply time < 5 minutes
- Can disable auto-reply
```

**US-007: Phát Hiện Lead**
```
AS A Small Business Owner
I WANT to detect buying intent from comments
SO THAT I can follow up with customers

Acceptance Criteria:
- Detects keywords ("giá bao nhiêu", "mua ở đâu")
- Auto-replies with CTA
- Highlights potential leads
```

**US-008: Auto-Like Comments**
```
AS A Content Creator
I WANT to auto-like comments
SO THAT followers feel acknowledged

Acceptance Criteria:
- Auto-likes new comments
- Excludes spam
- Rate limiting
```

---

### Epic 4: Settings

**US-009: Kết Nối Accounts**
```
AS A User
I WANT to connect my social media accounts
SO THAT I can post from one place

Acceptance Criteria:
- Connect Facebook, TikTok, YouTube, Zalo, Twitter
- OAuth2 flow
- Shows connection status
```

**US-010: Cấu Hình API Keys**
```
AS A User
I WANT to manage API keys
SO THAT the app can access services

Acceptance Criteria:
- Add/edit API keys (Gemini, Banana, etc.)
- Test connection
- Encrypted storage
```

---

## PHẦN 3: USER JOURNEYS

### Journey 1: Nga tạo content từ trend

```
1. [8:00 AM] Nga mở app
   → Sees: "🔥 Trend mới: #Recipe2025"

2. Click vào trend
   → Xem analysis của AI
   → Xem viral posts

3. Click "Tạo Content"
   → AI generates content cho FB, TikTok, YT
   → Suggests hashtags

4. Nga reviews và edits
   → Chọn Facebook và TikTok
   → Click "Đăng Ngay"

5. [8:05 AM] Posts go live
   → Comments start coming

6. AI auto-replies
   → "Giá bao nhiêu?" → "Inbox shop để nhận giá nhé!"

7. [5:00 PM] Nga checks
   → Facebook: 500 likes, 50 comments
   → 5 leads detected
```

### Journey 2: Minh responds to breaking trend

```
1. [2:00 PM] App notifies Minh
   → "⚡ Breaking trend detected!"

2. Minh clicks notification
   → Sees trend details

3. Click "Tạo Content"
   → AI generates quickly

4. Minh edits on the fly
   → Click "Post Now to Twitter"

5. [2:05 PM] Live on Twitter
   → 5 minutes from trend to post!
```

### Journey 3: Hùng bán hàng qua comments

```
1. Hùng posts product
   → "New product: Premium Headphones!"
   → Posts at 7 PM

2. [7:05 PM] Comments arrive
   → "Giá bao nhiêu?"
   → "Còn hàng không?"

3. AI replies instantly
   → "1,500,000đ ạ. Inbox nhận mã giảm 10%!"
   → "Còn ạ! Order ngay!"

4. Leads highlighted
   → Hùng contacts via Messenger
   → Closes sales

5. [9:00 PM] Results
   → 50 comments replied
   → 12 leads captured
   → 3 sales closed
```

---

## PHẦN 4: ARCHITECTURE

### 4.1 Simple Architecture

```
┌─────────────────────────────────────┐
│      WINFORMS APP (Desktop)          │
│  ┌────────────┐  ┌────────────┐     │
│  │ MainForm   │  │ Settings   │     │
│  └─────┬──────┘  └────────────┘     │
└────────┼─────────────────────────────┘
         │
┌────────▼─────────────────────────────┐
│      CORE SERVICES                    │
│  ┌──────────────┐  ┌──────────────┐  │
│  │TrendService  │  │ContentService│  │
│  └──────────────┘  └──────────────┘  │
│  ┌──────────────┐  ┌──────────────┐  │
│  │PostingService│  │CommentService│  │
│  └──────────────┘  └──────────────┘  │
└────────┬─────────────────────────────┘
         │
┌────────▼─────────────────────────────┐
│      PLATFORM CLIENTS                 │
│  [Facebook] [TikTok] [YouTube]        │
│  [Zalo] [Twitter]                     │
└────────┬─────────────────────────────┘
         │
┌────────▼─────────────────────────────┐
│      AI SERVICES                      │
│  [Gemini] [Banana] [Veo3]             │
└────────┬─────────────────────────────┘
         │
┌────────▼─────────────────────────────┐
│      DATA (SQLite)                    │
│  - Trends                             │
│  - Generated Content                  │
│  - Comments                           │
│  - Schedules                          │
└───────────────────────────────────────┘
```

### 4.2 Core Components

#### 1. TrendService
- Fetch trends from platforms
- Use Gemini to analyze
- Score and rank
- Store in SQLite

#### 2. ContentService
- Analyze viral content (Gemini)
- Generate content variations (Gemini)
- Create images (Banana API)
- Create videos (Veo 3 - optional)

#### 3. PostingService
- Post to platforms via OAuth2
- Schedule posts
- Track status
- Handle errors

#### 4. CommentService
- Monitor comments (polling)
- Analyze with Gemini
- Generate replies
- Auto-like
- Detect leads

---

### 4.3 Data Models

```csharp
public class TrendingTopic
{
    public int Id { get; set; }
    public string Platform { get; set; }
    public string Topic { get; set; }
    public int Score { get; set; }           // 0-100
    public DateTime DetectedAt { get; set; }
}

public class GeneratedContent
{
    public int Id { get; set; }
    public int TrendId { get; set; }
    public string Platform { get; set; }
    public string Text { get; set; }
    public string Hashtags { get; set; }
    public string Status { get; set; }       // Draft, Scheduled, Posted
    public DateTime? ScheduledFor { get; set; }
}

public class Comment
{
    public int Id { get; set; }
    public string PostId { get; set; }
    public string Platform { get; set; }
    public string Text { get; set; }
    public bool IsLead { get; set; }
    public bool Replied { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

---

## PHẦN 5: IMPLEMENTATION PLAN

### MVP - 8 Weeks

**Week 1-2: Foundation**
- Setup WinForms project
- SQLite database
- Basic UI (MainForm + SettingsForm)

**Week 3-4: OAuth2 & Platform Integration**
- OAuth2 for all 5 platforms
- Basic posting functionality
- Test with each platform

**Week 5-6: AI Integration**
- Gemini for trend detection
- Gemini for content generation
- Basic comment monitoring

**Week 7-8: Polish & Testing**
- Auto-reply logic
- Scheduling
- Bug fixes
- Testing

**MVP Deliverables:**
- ✅ Connect 5 platforms (FB, TikTok, YT, Zalo, X)
- ✅ Auto-detect trends
- ✅ AI generate content
- ✅ Post to platforms
- ✅ AI reply comments
- ✅ Basic scheduling

---

## PHẦN 6: TECH STACK

### Desktop App
- **Language**: C# .NET 6
- **UI**: WinForms
- **Database**: SQLite + Entity Framework Core

### APIs
- **Platforms**: Facebook Graph API, TikTok API, YouTube Data API, Zalo API, Twitter API
- **AI**: Google Gemini API, Banana API, Veo 3 API (optional)

### Libraries
- Newtonsoft.Json
- Microsoft.EntityFrameworkCore.Sqlite
- Google.Apis.YouTube.v3
- Google.GenerativeAI

---

## PHẦN 7: COST ESTIMATE

**Monthly (Moderate Usage)**:
- Google Gemini: $150
- Banana API: $100
- Veo 3: $200 (optional)

**Total**: $250-450/month

---

## CONCLUSION

### Scope Đơn Giản

**4 Chức Năng Chính**:
1. Tìm trends (Gemini + Google Search)
2. Tạo content (Gemini + Banana + Veo)
3. Đăng bài (5 platforms)
4. Reply comments (Gemini)

**KHÔNG LÀM**:
- ❌ Distributed workers
- ❌ Team collaboration
- ❌ Advanced analytics
- ❌ A/B testing
- ❌ Mobile/Web version

**Kiến Trúc**:
- 1 app WinForms
- SQLite database
- Chạy trên 1 máy
- Single user

**Timeline**: 8 tuần MVP
**Budget**: $10K development + $250-450/month runtime

🚀 **Đơn giản, thực tế, dễ implement!**
