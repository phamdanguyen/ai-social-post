# Nghiên Cứu API Mạng Xã Hội và Hệ Thống Auto Post

## Tổng Quan Dự Án

Mục tiêu: Xây dựng hệ thống tự động tìm kiếm nội dung trending/viral, sử dụng AI để phân tích và viết lại nội dung, sau đó tự động đăng lên các kênh mạng xã hội.

**Các nền tảng mục tiêu:**
- Facebook
- Zalo
- YouTube
- TikTok

---

## 1. FACEBOOK GRAPH API

### 1.1 Thông Tin Cơ Bản
- **Phiên bản hiện tại (2025)**: Graph API v22.0
- **API chính**: `/page_id/feed`
- **Phương thức**: POST để đăng, GET để đọc

### 1.2 Tính Năng Đăng Bài

#### Endpoint Đăng Bài
```
POST https://graph.facebook.com/v22.0/{page_id}/feed
```

#### Tham Số Cơ Bản
- `message`: Nội dung text của bài đăng
- `link`: URL đính kèm
- `access_token`: Page access token (bắt buộc)

#### Ví Dụ Code
```javascript
const pageId = 'YOUR_PAGE_ID';
const pageToken = 'YOUR_PAGE_TOKEN';

fetch(`https://graph.facebook.com/v22.0/${pageId}/feed`, {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
  },
  body: JSON.stringify({
    message: 'Nội dung bài viết',
    link: 'https://example.com',
    access_token: pageToken
  })
});
```

### 1.3 Quyền Truy Cập & Permissions

#### Các Permission Cần Thiết
- `pages_manage_posts`: Để đăng bài lên Page
- `pages_read_engagement`: Để đọc engagement metrics
- `Page Public Content Access`: Để truy cập nội dung công khai

#### Loại Token
- **Page Access Token**: Bắt buộc để đăng bài với tư cách Page
- **User Access Token**: Để xem bài viết mà user đó có quyền xem
- **App Access Token**: Cho các thao tác cấp ứng dụng

### 1.4 Đọc Feed và Phân Tích

#### Endpoint Đọc Feed
```
GET https://graph.facebook.com/v22.0/{page_id}/feed
```

#### Dữ Liệu Trả Về
```json
{
  "data": [
    {
      "id": "post_id",
      "message": "Nội dung bài viết",
      "created_time": "2025-01-15T10:30:00+0000",
      "likes": { "count": 150 },
      "comments": { "count": 25 },
      "shares": { "count": 10 }
    }
  ]
}
```

### 1.5 Hạn Chế & Lưu Ý

- **Rate Limiting**: API có giới hạn số lượng request/giờ
- **App Review**: Cần phê duyệt ứng dụng để sử dụng đầy đủ permissions
- **Chính Sách Nội Dung**: Phải tuân thủ Community Standards của Facebook
- **Token Expiration**: Page token cần được làm mới định kỳ

---

## 2. ZALO API

### 2.1 Thông Tin Cơ Bản
- **Trang chủ**: https://developers.zalo.me/docs
- **Hai loại API chính**:
  1. Social API
  2. Official Account API (OA API)

### 2.2 Social API

#### Chức Năng
- Truy cập dữ liệu người dùng Zalo
- Truy cập danh sách bạn bè
- Đăng thông tin mới
- Sử dụng giao thức HTTP

#### Use Cases
- Đăng nhập qua Zalo
- Lấy thông tin profile người dùng
- Chia sẻ nội dung lên Zalo

### 2.3 Official Account API (OA API)

#### Chức Năng Chính
- Tương tác với người dùng thay mặt OA
- Gửi tin nhắn tự động
- Quản lý người quan tâm (followers)
- Sử dụng giao thức HTTPS

#### Tính Năng Đăng Bài
- Gửi tin nhắn văn bản
- Gửi hình ảnh, video
- Gửi tin nhắn tương tác (interactive messages)
- Gửi tin nhắn template

### 2.4 SDKs và Libraries

#### PHP SDK
```bash
composer require zaloplatform/zalo-php-sdk
```

#### Node.js SDK
```bash
npm install zalo-sdk
# hoặc
npm install zalo-api
```

#### Python (Unofficial)
```bash
pip install zlapi
```

### 2.5 Quy Trình Tích Hợp

1. **Đăng ký ứng dụng** tại developers.zalo.me
2. **Lấy App ID và Secret Key**
3. **Xác thực OAuth 2.0**
4. **Lấy Access Token**
5. **Gọi API** để đăng bài/gửi tin nhắn

### 2.6 Hạn Chế & Lưu Ý

- **Xác minh OA**: Cần xác minh Official Account để mở khóa đầy đủ tính năng
- **Giới hạn tin nhắn**: Có quota tin nhắn theo từng loại tài khoản
- **Nội dung**: Phải tuân thủ quy định của Zalo về nội dung
- **Tài liệu tiếng Việt**: Tài liệu chủ yếu bằng tiếng Việt

---

## 3. YOUTUBE DATA API v3

### 3.1 Thông Tin Cơ Bản
- **Phiên bản hiện tại**: YouTube Data API v3
- **API chính**: `videos.insert`
- **Giới hạn upload**: Tối đa 256 GB/video
- **Formats hỗ trợ**: video/*

### 3.2 Quota và Chi Phí

#### Hệ Thống Quota
- **Quota mặc định**: 10,000 units/ngày
- **Reset**: Nửa đêm theo giờ Pacific Time (PT)
- **Chi phí videos.insert**: 1,600 units

#### Ví Dụ Tính Toán
- 10,000 units / 1,600 units = ~6 video uploads/ngày
- Để upload nhiều hơn cần request tăng quota

### 3.3 Xác Thực OAuth 2.0

#### Quy Trình Setup
1. Tạo project trong Google Cloud Console
2. Enable YouTube Data API v3
3. Tạo OAuth 2.0 credentials
4. Download `client_secrets.json`
5. Xác thực và lấy refresh token
6. Sử dụng refresh token để lấy access token

#### Lưu Ý Quan Trọng
- **Không dùng Service Accounts**: YouTube API không hỗ trợ service accounts
- **User Authorization**: Cần authorization của user sở hữu channel
- **Refresh Token**: Lưu refresh token để tự động renew access token

### 3.4 Upload Video

#### Endpoint
```
POST https://www.googleapis.com/upload/youtube/v3/videos
```

#### Tham Số
- `part`: snippet,status (bắt buộc)
- `snippet.title`: Tiêu đề video
- `snippet.description`: Mô tả video
- `snippet.tags`: Array các tag
- `status.privacyStatus`: public, unlisted, hoặc private

#### Ví Dụ Python
```python
from googleapiclient.discovery import build
from googleapiclient.http import MediaFileUpload

youtube = build('youtube', 'v3', credentials=credentials)

request = youtube.videos().insert(
    part="snippet,status",
    body={
        "snippet": {
            "title": "Tiêu đề video",
            "description": "Mô tả video",
            "tags": ["tag1", "tag2"],
            "categoryId": "22"
        },
        "status": {
            "privacyStatus": "public"
        }
    },
    media_body=MediaFileUpload("video.mp4")
)

response = request.execute()
```

### 3.5 Thêm Thumbnail và Captions

#### Thêm Thumbnail
```
POST https://www.googleapis.com/upload/youtube/v3/thumbnails/set
```

#### Thêm Captions
```
POST https://www.googleapis.com/upload/youtube/v3/captions
```

### 3.6 Personal Use Option

**Cho sử dụng cá nhân:**
- Không cần app verification nếu chỉ có 1 user
- Setup đơn giản hơn
- Phù hợp cho automation cá nhân

### 3.7 Hạn Chế & Lưu Ý

- **Quota hạn chế**: Chỉ 6 videos/ngày với quota mặc định
- **Verification**: Cần verify app để tăng quota và public app
- **Processing time**: Video cần thời gian processing sau upload
- **Copyright**: Cần đảm bảo không vi phạm bản quyền

---

## 4. TIKTOK CONTENT POSTING API

### 4.1 Thông Tin Cơ Bản
- **Trang chủ**: https://developers.tiktok.com/
- **Sản phẩm**: Content Posting API
- **Hai chế độ chính**:
  1. Direct Post API
  2. Upload API (draft mode)

### 4.2 Direct Post API

#### Tính Năng
- Đăng video trực tiếp lên TikTok profile
- Hỗ trợ đầy đủ posting settings:
  - Caption
  - Hashtags
  - Privacy settings
  - Allow comments/duet/stitch
  - Branded content toggle

#### Endpoint
```
POST https://open.tiktokapis.com/v2/post/publish/video/init/
```

### 4.3 Hai Phương Thức Upload

#### 1. FILE_UPLOAD
- Upload file video trực tiếp
- Phù hợp cho video đã có sẵn

#### 2. PULL_FROM_URL
- TikTok pull video từ URL của bạn
- Cần verify domain
- Phù hợp cho video hosted trên server riêng

### 4.4 Quy Trình Đăng Video

1. **Initialize Upload**
   ```
   POST /v2/post/publish/video/init/
   ```
   - Nhận upload_id và upload_url

2. **Upload Video**
   ```
   PUT {upload_url}
   ```
   - Upload binary video data

3. **Publish Video**
   ```
   POST /v2/post/publish/status/fetch/
   ```
   - Check status và lấy video info

### 4.5 Yêu Cầu và Permissions

#### Đăng Ký App
1. Tạo app tại developers.tiktok.com
2. Add Content Posting API product
3. Enable Direct Post configuration
4. Submit for review (nếu cần public)

#### Permission Scope
- `video.publish`: Bắt buộc để đăng video
- `video.upload`: Để upload draft video

### 4.6 Giới Hạn Quan Trọng ⚠️

#### Unaudited Clients
- **QUAN TRỌNG**: Tất cả video từ unaudited clients sẽ bị restrict về private mode
- Cần audit app để content được public
- Audit process verify tuân thủ Terms of Service

#### Rate Limits
- Có giới hạn số lượng video post/ngày
- Có giới hạn request/giờ

### 4.7 Ví Dụ Code

```javascript
// Initialize upload
const initResponse = await fetch('https://open.tiktokapis.com/v2/post/publish/video/init/', {
  method: 'POST',
  headers: {
    'Authorization': `Bearer ${accessToken}`,
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    post_info: {
      title: "Video title",
      privacy_level: "PUBLIC_TO_EVERYONE",
      disable_duet: false,
      disable_comment: false,
      disable_stitch: false
    },
    source_info: {
      source: "FILE_UPLOAD",
      video_size: videoSize,
      chunk_size: chunkSize,
      total_chunk_count: totalChunks
    }
  })
});

// Upload video chunks
const uploadResponse = await fetch(uploadUrl, {
  method: 'PUT',
  headers: {
    'Content-Type': 'video/mp4',
    'Content-Range': `bytes ${start}-${end}/${totalSize}`
  },
  body: videoChunk
});

// Publish
const publishResponse = await fetch('https://open.tiktokapis.com/v2/post/publish/status/fetch/', {
  method: 'POST',
  headers: {
    'Authorization': `Bearer ${accessToken}`,
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    publish_id: publishId
  })
});
```

### 4.8 Upload API (Draft Mode)

- Creator upload video dưới dạng draft
- Có thể edit thêm trong TikTok app
- Phù hợp cho workflow cần human review

### 4.9 Hạn Chế & Lưu Ý

- **App Audit**: Bắt buộc để video được public
- **Domain Verification**: Cần verify domain cho PULL_FROM_URL
- **Video Requirements**:
  - Format: MP4
  - Duration: 3 giây - 10 phút
  - Size: Tối đa 4GB
  - Resolution: Tối thiểu 720p
- **Compliance**: Phải tuân thủ TikTok Community Guidelines

---

## 5. PHÁT HIỆN NỘI DUNG TRENDING/VIRAL

### 5.1 APIs và Tools

#### TikTok Search/Trends API
**Provider**: Data365, Phyllo, EchoTik

**Tính năng:**
- Fetch top trending videos real-time
- Theo dõi trending hashtags
- Lấy engagement metrics (likes, views, shares)
- Filter theo region, language, popularity
- Predictive analytics cho trend sắp lên

**Ví dụ use case:**
```javascript
// Lấy trending videos
const trendingVideos = await tiktokAPI.getTrending({
  region: 'VN',
  category: 'entertainment',
  sortBy: 'engagement_rate',
  limit: 50
});
```

#### Google Trends API
**Tích hợp với n8n workflow:**
- Tự động discover trending Google queries
- Chuyển đổi thành content ideas
- Kết hợp với AI để generate posts

#### Social Media Trend Scraper
**Tool**: Apify 6-in-1 Scraper

**Platforms hỗ trợ:**
- TikTok
- Instagram
- YouTube
- Reddit
- Twitter/X
- Pinterest

**Features:**
- AI analysis
- Viral hashtags detection
- Engagement insights
- Actionable recommendations

### 5.2 MediaMiner API

**Chuyên về:**
- Content discovery
- Trend analysis
- Hashtag tracking
- Influencer identification
- Viral content detection

**Điểm mạnh:**
- Predictive analytics
- Detect trending content sớm (trước khi peak)
- Cross-platform analysis

### 5.3 Real-Time Detection

**Latency:**
- Các API tốt có latency < 5 phút
- Near-instantaneous data delivery
- Cho phép react nhanh với trends

**Benefits:**
- Detect crisis situations
- Catch viral moments sớm
- Optimize timing cho posts

### 5.4 Chiến Lược Tối Ưu

#### 1. Multi-Source Monitoring
```
Google Trends + TikTok API + Reddit API + Twitter API
→ Cross-validate trends
→ Identify real vs fake trends
```

#### 2. Engagement Metrics Tracking
- Like rate
- Share rate
- Comment sentiment
- Growth velocity
- Virality coefficient

#### 3. Regional Targeting
- Theo dõi trends theo region (VN specific)
- Language-specific content
- Cultural relevance check

### 5.5 Automation với n8n

**Workflow Example:**
1. Trigger: Cron job mỗi 15 phút
2. Fetch: Google Trends + Social APIs
3. Filter: Engagement threshold + relevance score
4. Analyze: AI analysis (Perplexity/Claude)
5. Store: Database với trend data
6. Alert: Notify khi có hot trend

### 5.6 ROI và Impact

**Số liệu thực tế:**
- Engagement tăng 30-50% khi align với trends
- Response time < 1 giờ cho hot trends
- Best posting time: Trong 2-6 giờ đầu của trend

---

## 6. AI CONTENT ANALYSIS & REWRITING

### 6.1 Video Analysis Workflow

#### Limitations
- **Claude AI**: Không thể analyze video trực tiếp
- **Giải pháp**: Chuyển video thành text transcript trước

#### Pipeline Hoàn Chỉnh

**Step 1: Video → Audio → Transcript**
```python
# Sử dụng OpenAI Whisper
import whisper

model = whisper.load_model("large-v3")
result = model.transcribe("video.mp4", language="vi")
transcript = result["text"]
```

**Step 2: Transcript → Analysis**
```python
# Sử dụng Claude API
import anthropic

client = anthropic.Anthropic(api_key="your-api-key")

analysis = client.messages.create(
    model="claude-sonnet-4-5-20250929",
    max_tokens=4000,
    messages=[{
        "role": "user",
        "content": f"""
        Phân tích transcript này và trích xuất:
        1. Main topics
        2. Key messages
        3. Emotional tone
        4. Viral elements
        5. Target audience

        Transcript: {transcript}
        """
    }]
)
```

**Step 3: Content Rewriting**
```python
rewritten = client.messages.create(
    model="claude-sonnet-4-5-20250929",
    max_tokens=2000,
    messages=[{
        "role": "user",
        "content": f"""
        Dựa trên analysis này, viết lại content cho Facebook post:
        - Giữ main message
        - Optimize cho engagement
        - Thêm call-to-action
        - Suggest hashtags

        Analysis: {analysis}
        """
    }]
)
```

### 6.2 Tools và Platforms

#### OpenAI Whisper
- **Open source**, chạy local
- Hỗ trợ 99+ languages
- Accuracy cao cho tiếng Việt
- Models: tiny, base, small, medium, large
- Large-v3: Accuracy tốt nhất

#### Claude (Anthropic)
- **Strengths**: Content repurposing, summarization
- Model recommend: claude-sonnet-4-5 cho balance giá/chất lượng
- Context window: 200K tokens
- Tốt cho Vietnamese content

#### OpenAI GPT-4
- **Strengths**: Creative rewriting, style adaptation
- GPT-4 Turbo: Cost-effective
- Vision capability: Analyze thumbnails, images

#### Video Understanding APIs

**Cho Visual Analysis:**
- **OpenAI GPT-4V**: Multimodal, analyze video frames
- **Amazon Rekognition**: Object/face detection, activity recognition
- **Microsoft Azure Video Indexer**: Comprehensive video analysis
- **Google Cloud Video Intelligence**: Scene detection, label detection

### 6.3 Content Repurposing Strategies

#### 1. Video → Blog Post
**Tools**: YouTube + Whisper + Claude
**Process**:
- Extract transcript
- Identify sections
- Expand với research
- Add images/screenshots
- Format for web

**Cost**: ~$4 cho 2-hour video (Claude Opus)

#### 2. Long Video → Short Clips
**Tools**: FFmpeg + GPT-4V
**Process**:
- Analyze video scenes
- Identify highlight moments
- Auto-cut clips
- Add captions
- Optimize for TikTok/Reels

#### 3. Article → Video Script
**Tools**: Claude/GPT-4
**Process**:
- Summarize article
- Create scene-by-scene script
- Suggest visuals
- Add voice-over text

### 6.4 AI Image/Video Generation

#### Stable Diffusion / DALL-E
- Generate thumbnails
- Create social media graphics
- Style consistency

#### Runway ML / Pika
- Text-to-video generation
- Video editing with AI
- Style transfer

#### ElevenLabs
- Voice cloning
- Text-to-speech
- Multi-language support

### 6.5 Content Quality Checks

#### AI Detector Tools
- Detect AI-generated content
- 98% accuracy
- Support: GPT, Claude, Gemini
- Ensure natural-sounding output

#### Human-in-the-Loop
- AI tạo draft
- Human review và edit
- Ensure brand voice
- Cultural appropriateness check

### 6.6 Automation Workflow

```javascript
// Complete workflow
async function processViralVideo(videoUrl) {
  // 1. Download video
  const video = await downloadVideo(videoUrl);

  // 2. Extract transcript
  const transcript = await whisperAPI.transcribe(video);

  // 3. Analyze with Claude
  const analysis = await claudeAPI.analyze(transcript);

  // 4. Generate variations
  const variations = await Promise.all([
    claudeAPI.rewriteFor('facebook', analysis),
    claudeAPI.rewriteFor('tiktok', analysis),
    claudeAPI.rewriteFor('youtube', analysis)
  ]);

  // 5. Generate visuals
  const thumbnail = await dalleAPI.generate(analysis.keyTopics);

  // 6. Create video clips
  const clips = await extractHighlights(video, analysis.highlights);

  return {
    original: video,
    analysis,
    variations,
    thumbnail,
    clips
  };
}
```

---

## 7. AUTO-NURTURING FANPAGE & FEED

### 7.1 Chiến Lược Nuôi Fanpage

#### Mục Tiêu
- Tăng followers organic
- Tăng engagement rate
- Build community
- Maintain consistent presence

#### Key Metrics
- Follower growth rate
- Engagement rate (likes, comments, shares)
- Reach và impressions
- Click-through rate
- Conversion rate

### 7.2 Fan Page Robot

**Platform**: fanpagerobot.com

**Features:**
- 10-in-1 marketing automation
- Auto-discover hot content
- Auto-post to multiple platforms
- Schedule posts
- Analytics dashboard

**Use Cases:**
- Marketing agencies quản lý multiple clients
- Auto-curate content từ sources
- Cross-post to blogs và social media

### 7.3 Content Automation Strategy

#### 1. Content Calendar
```javascript
const contentPlan = {
  monday: ['motivational', 'industry-news'],
  tuesday: ['tutorial', 'tips'],
  wednesday: ['case-study', 'behind-scenes'],
  thursday: ['user-generated-content', 'testimonial'],
  friday: ['entertainment', 'memes'],
  saturday: ['lifestyle', 'community'],
  sunday: ['inspiration', 'weekly-recap']
};
```

#### 2. Posting Frequency
- **Minimum**: 1 post/day
- **Optimal**: 2-3 posts/day
- **Peak times**:
  - Morning: 7-9 AM
  - Lunch: 12-2 PM
  - Evening: 7-9 PM

#### 3. Content Mix (80-20 Rule)
- 80% valuable/educational content
- 20% promotional content

### 7.4 Lead Nurturing Automation

#### ManyChat
**Platform**: Conversational marketing

**Features:**
- Auto-reply messages
- Chat sequences
- Lead qualification
- Appointment booking
- E-commerce integration

**Workflow Example:**
1. User comments "QUAN TÂM"
2. Auto-send Messenger message
3. Qualify lead with questions
4. Send relevant content
5. Schedule follow-up
6. Convert to customer

#### Zapier/n8n Integration
```javascript
// n8n workflow
{
  trigger: 'New Facebook comment',
  actions: [
    'Check sentiment',
    'Reply with template',
    'Add to CRM',
    'Trigger nurture sequence',
    'Schedule follow-up'
  ]
}
```

### 7.5 Popular Automation Tools

#### Metricool
- Multi-platform scheduling
- Analytics
- Best time to post
- Competitor analysis
- Content calendar

#### Hootsuite
- Social listening
- Team collaboration
- Bulk scheduling
- ROI tracking

#### Buffer
- Queue management
- Analytics
- Browser extension
- RSS feeds integration

#### MeetEdgar
- Content recycling
- Category-based scheduling
- Variations testing
- Evergreen content library

#### Sprout Social
- Advanced analytics
- CRM integration
- Team workflow
- Reporting

### 7.6 AI-Powered Content Strategy

#### Content Ideation
- AI suggests topics based on trends
- Analyze top-performing content
- Generate content calendar

#### Caption Generation
- Auto-write captions
- Multiple variations
- A/B testing

#### Optimal Timing
- AI predicts best posting times
- Based on audience behavior
- Platform-specific optimization

#### Performance Analysis
- Track what works
- Identify patterns
- Adjust strategy automatically

### 7.7 Community Management

#### Auto-Responses
```javascript
const autoResponses = {
  'giá bao nhiêu': 'responseTemplate.pricing',
  'còn hàng không': 'responseTemplate.availability',
  'ship như thế nào': 'responseTemplate.shipping',
  // ... more patterns
};

// Tự động reply khi detect keywords
```

#### Sentiment Analysis
- Detect negative comments → Priority response
- Identify brand advocates → Engage more
- Track sentiment trends

#### Engagement Automation
- Auto-like comments
- Auto-reply common questions
- Tag team members for complex queries

### 7.8 Cross-Platform Strategy

#### Content Adaptation
**Same core message, different formats:**
- Facebook: Longer text + image
- TikTok: Short video + trendy music
- YouTube: Long-form + detailed
- Zalo: Concise + action-oriented

#### Timing Coordination
```javascript
const crossPlatformSchedule = {
  'Monday 8AM': {
    facebook: 'detailed-post',
    tiktok: 'short-video',
    youtube: 'schedule-for-evening',
    zalo: 'send-to-followers'
  }
};
```

### 7.9 Growth Hacking Tactics

#### 1. Contest/Giveaway Automation
- Auto-pick winners
- Verify participants
- Send notifications
- Track ROI

#### 2. User-Generated Content
- Auto-detect mentions
- Request permission
- Repost with credit
- Engage with creators

#### 3. Influencer Collaboration
- Track relevant influencers
- Monitor mentions
- Auto-engage
- Track campaign performance

#### 4. Viral Loop Creation
- Referral mechanics
- Share incentives
- Progress tracking
- Reward distribution

---

## 8. KIẾN TRÚC HỆ THỐNG ĐỀ XUẤT

### 8.1 Tổng Quan

```
┌─────────────────────────────────────────────────────────────┐
│                    TRENDING DETECTOR                         │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐   │
│  │  TikTok  │  │  Google  │  │ YouTube  │  │  Reddit  │   │
│  │  Trends  │  │  Trends  │  │  Trends  │  │  Trends  │   │
│  └─────┬────┘  └─────┬────┘  └─────┬────┘  └─────┬────┘   │
│        └──────────────┴─────────────┴─────────────┘         │
│                           │                                  │
└───────────────────────────┼──────────────────────────────────┘
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                    CONTENT ANALYZER                          │
│  ┌────────────────────────────────────────────────────┐     │
│  │  • Download Video/Content                          │     │
│  │  • Extract Transcript (Whisper)                    │     │
│  │  • Analyze Content (Claude/GPT-4)                  │     │
│  │  • Extract Key Topics & Viral Elements             │     │
│  │  • Generate Insights                               │     │
│  └────────────────────────────────────────────────────┘     │
└───────────────────────────┬──────────────────────────────────┘
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                   CONTENT GENERATOR                          │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │   Rewrite    │  │   Generate   │  │    Create    │      │
│  │    Text      │  │    Images    │  │    Videos    │      │
│  │   (Claude)   │  │  (DALL-E/SD) │  │  (Runway ML) │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
└───────────────────────────┬──────────────────────────────────┘
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                 PLATFORM ADAPTOR                             │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  • Adapt content for each platform                   │   │
│  │  • Optimize format, length, style                    │   │
│  │  • Generate hashtags                                 │   │
│  │  • Add call-to-actions                               │   │
│  └──────────────────────────────────────────────────────┘   │
└───────────────────────────┬──────────────────────────────────┘
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                   AUTO POSTER                                │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐   │
│  │ Facebook │  │   Zalo   │  │ YouTube  │  │  TikTok  │   │
│  │   API    │  │   API    │  │   API    │  │   API    │   │
│  └──────────┘  └──────────┘  └──────────┘  └──────────┘   │
└───────────────────────────┬──────────────────────────────────┘
                            ▼
┌─────────────────────────────────────────────────────────────┐
│              ANALYTICS & OPTIMIZATION                        │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  • Track performance                                 │   │
│  │  • A/B testing                                       │   │
│  │  • Learn from successful posts                      │   │
│  │  • Optimize timing & content                        │   │
│  └──────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
```

### 8.2 Tech Stack Đề Xuất

#### Backend
- **Node.js/Python**: Main application
- **PostgreSQL**: Database cho content, analytics
- **Redis**: Caching và queue management
- **Bull/BullMQ**: Job queue cho async tasks

#### AI Services
- **OpenAI Whisper**: Transcript generation
- **Claude API**: Content analysis & rewriting
- **OpenAI GPT-4**: Alternative content generation
- **DALL-E/Stable Diffusion**: Image generation
- **Runway ML**: Video generation

#### Social Media APIs
- **Facebook Graph API**
- **Zalo SDK**
- **YouTube Data API**
- **TikTok Content Posting API**

#### Automation
- **n8n**: Workflow automation (self-hosted)
- **Cron jobs**: Scheduled tasks

#### Monitoring
- **Sentry**: Error tracking
- **Prometheus + Grafana**: Metrics
- **ELK Stack**: Logging

### 8.3 Database Schema

```sql
-- Trending Topics
CREATE TABLE trending_topics (
  id UUID PRIMARY KEY,
  platform VARCHAR(50),
  topic TEXT,
  hashtags JSONB,
  engagement_score INTEGER,
  first_seen TIMESTAMP,
  peak_time TIMESTAMP,
  status VARCHAR(20), -- rising, peak, declining
  metadata JSONB
);

-- Content Sources
CREATE TABLE content_sources (
  id UUID PRIMARY KEY,
  url TEXT,
  platform VARCHAR(50),
  original_content JSONB,
  transcript TEXT,
  download_date TIMESTAMP,
  trending_topic_id UUID REFERENCES trending_topics(id)
);

-- Analyzed Content
CREATE TABLE analyzed_content (
  id UUID PRIMARY KEY,
  source_id UUID REFERENCES content_sources(id),
  analysis JSONB, -- key topics, tone, target audience
  viral_elements JSONB,
  rewrite_suggestions JSONB,
  analyzed_at TIMESTAMP
);

-- Generated Content
CREATE TABLE generated_content (
  id UUID PRIMARY KEY,
  analyzed_content_id UUID REFERENCES analyzed_content(id),
  platform VARCHAR(50),
  content_type VARCHAR(50), -- text, image, video
  content JSONB,
  hashtags JSONB,
  status VARCHAR(20), -- draft, scheduled, posted
  scheduled_for TIMESTAMP,
  created_at TIMESTAMP
);

-- Posted Content
CREATE TABLE posted_content (
  id UUID PRIMARY KEY,
  generated_content_id UUID REFERENCES generated_content(id),
  platform VARCHAR(50),
  platform_post_id VARCHAR(255),
  posted_at TIMESTAMP,
  initial_metrics JSONB
);

-- Performance Metrics
CREATE TABLE performance_metrics (
  id UUID PRIMARY KEY,
  posted_content_id UUID REFERENCES posted_content(id),
  metrics JSONB, -- likes, shares, comments, views
  collected_at TIMESTAMP
);
```

### 8.4 Workflow Chi Tiết

#### 1. Trend Detection (Chạy mỗi 15 phút)
```javascript
async function detectTrends() {
  // Fetch từ multiple sources
  const [tiktokTrends, googleTrends, youtubeTrends] = await Promise.all([
    tiktokAPI.getTrending(),
    googleTrendsAPI.getTrending('VN'),
    youtubeAPI.getTrending('VN')
  ]);

  // Merge và score
  const allTrends = mergeTrends([tiktokTrends, googleTrends, youtubeTrends]);

  // Filter theo threshold
  const hotTrends = allTrends.filter(t => t.score > THRESHOLD);

  // Save to DB
  await saveTrends(hotTrends);

  // Trigger content analysis cho top trends
  for (const trend of hotTrends.slice(0, 10)) {
    await queue.add('analyze-trend', { trendId: trend.id });
  }
}
```

#### 2. Content Analysis (Queue job)
```javascript
async function analyzeTrend(trendId) {
  const trend = await db.getTrend(trendId);

  // Find top content cho trend này
  const topContent = await findTopContent(trend);

  for (const content of topContent) {
    // Download
    const file = await downloadContent(content.url);

    // Extract transcript nếu là video
    let transcript;
    if (content.type === 'video') {
      transcript = await whisperAPI.transcribe(file);
    } else {
      transcript = await extractText(file);
    }

    // Analyze với Claude
    const analysis = await claudeAPI.analyze({
      transcript,
      trend: trend.topic,
      platform: content.platform
    });

    // Save
    await db.saveAnalysis({
      contentId: content.id,
      analysis,
      transcript
    });

    // Trigger content generation
    await queue.add('generate-content', {
      analysisId: analysis.id
    });
  }
}
```

#### 3. Content Generation (Queue job)
```javascript
async function generateContent(analysisId) {
  const analysis = await db.getAnalysis(analysisId);

  // Generate cho từng platform
  const platforms = ['facebook', 'tiktok', 'youtube', 'zalo'];

  for (const platform of platforms) {
    // Text content
    const text = await claudeAPI.rewrite({
      analysis,
      platform,
      style: 'engaging'
    });

    // Generate hashtags
    const hashtags = await generateHashtags(analysis, platform);

    // Generate images nếu cần
    let images;
    if (platform === 'facebook' || platform === 'zalo') {
      images = await dalleAPI.generate({
        prompt: analysis.keyTopics.join(', '),
        style: 'photorealistic'
      });
    }

    // Generate video nếu cần
    let video;
    if (platform === 'tiktok') {
      video = await createShortVideo(analysis);
    }

    // Save generated content
    await db.saveGeneratedContent({
      analysisId,
      platform,
      text,
      hashtags,
      images,
      video,
      status: 'ready_to_schedule'
    });
  }
}
```

#### 4. Scheduling & Posting
```javascript
async function scheduleContent() {
  const readyContent = await db.getReadyContent();

  for (const content of readyContent) {
    // Determine optimal time
    const optimalTime = await predictBestTime({
      platform: content.platform,
      contentType: content.type,
      targetAudience: content.targetAudience
    });

    // Schedule
    await db.scheduleContent(content.id, optimalTime);

    // Add to posting queue
    await queue.add('post-content', {
      contentId: content.id
    }, {
      delay: optimalTime - Date.now()
    });
  }
}

async function postContent(contentId) {
  const content = await db.getContent(contentId);

  try {
    let result;

    switch (content.platform) {
      case 'facebook':
        result = await facebookAPI.post(content);
        break;
      case 'zalo':
        result = await zaloAPI.post(content);
        break;
      case 'youtube':
        result = await youtubeAPI.upload(content);
        break;
      case 'tiktok':
        result = await tiktokAPI.post(content);
        break;
    }

    // Save posted content info
    await db.savePostedContent({
      contentId,
      platform: content.platform,
      platformPostId: result.id,
      postedAt: new Date()
    });

    // Schedule metrics collection
    await queue.add('collect-metrics', {
      postedContentId: result.id
    }, {
      delay: 3600000 // 1 hour later
    });

  } catch (error) {
    // Handle errors, retry logic
    await handlePostError(contentId, error);
  }
}
```

#### 5. Analytics Collection
```javascript
async function collectMetrics(postedContentId) {
  const posted = await db.getPostedContent(postedContentId);

  let metrics;

  switch (posted.platform) {
    case 'facebook':
      metrics = await facebookAPI.getMetrics(posted.platformPostId);
      break;
    case 'tiktok':
      metrics = await tiktokAPI.getMetrics(posted.platformPostId);
      break;
    // ... other platforms
  }

  await db.saveMetrics({
    postedContentId,
    metrics,
    collectedAt: new Date()
  });

  // Schedule next collection (hourly for first 24h)
  if (isWithin24Hours(posted.postedAt)) {
    await queue.add('collect-metrics', {
      postedContentId
    }, {
      delay: 3600000 // 1 hour
    });
  }
}
```

### 8.5 Configuration Management

```javascript
// config/platforms.js
module.exports = {
  facebook: {
    apiVersion: 'v22.0',
    maxPostLength: 63206,
    supportedMediaTypes: ['image', 'video', 'link'],
    optimalImageSize: { width: 1200, height: 630 },
    rateLimit: {
      postsPerDay: 100,
      postsPerHour: 25
    }
  },

  tiktok: {
    maxVideoSize: 4 * 1024 * 1024 * 1024, // 4GB
    maxVideoDuration: 600, // 10 minutes
    minVideoDuration: 3,
    requiredResolution: '720p',
    rateLimit: {
      postsPerDay: 10,
      postsPerHour: 3
    }
  },

  youtube: {
    quotaPerDay: 10000,
    uploadCost: 1600,
    maxVideosPerDay: 6,
    maxVideoSize: 256 * 1024 * 1024 * 1024, // 256GB
    supportedFormats: ['mp4', 'mov', 'avi', 'wmv']
  },

  zalo: {
    messageTypes: ['text', 'image', 'video', 'template'],
    maxTextLength: 2000,
    requiresOA: true,
    rateLimit: {
      messagesPerDay: 1000
    }
  }
};
```

---

## 9. TUÂN THỦ & RỦI RO

### 9.1 Các Vấn Đề Pháp Lý

#### Bản Quyền Nội Dung
- ⚠️ **Không được** copy 100% nội dung gốc
- ✅ **Được phép** rewrite với substantial changes
- ✅ **Được phép** tham khảo và tạo nội dung mới
- ⚠️ Cần credit nguồn nếu reference trực tiếp

#### DMCA & Copyright Claims
- YouTube: ContentID system tự động detect
- Facebook: Rights Manager
- TikTok: Copyright detection AI
- **Giải pháp**: Rewrite hoàn toàn, tạo visuals mới

#### Privacy & Personal Data
- Tuân thủ GDPR nếu có EU users
- Tuân thủ PDPA (Vietnam)
- Không scrape personal information
- Xin phép trước khi repost UGC

### 9.2 Platform Policies

#### Facebook Community Standards
- Không fake news
- Không spam
- Không clickbait
- Không hate speech
- Rate limit tuân thủ

#### TikTok Community Guidelines
- No copyrighted music (trừ TikTok library)
- No violent/graphic content
- No misleading information
- Original content encouraged

#### YouTube Policies
- Copyright strikes (3 strikes = ban)
- Monetization requirements
- Community guidelines
- Advertiser-friendly content

### 9.3 API Usage Compliance

#### Rate Limiting
```javascript
// Implement rate limiter
const rateLimiter = {
  facebook: new RateLimiter(25, 'hour'),
  tiktok: new RateLimiter(3, 'hour'),
  youtube: new RateLimiter(6, 'day')
};

async function post(platform, content) {
  await rateLimiter[platform].wait();
  return await platformAPI[platform].post(content);
}
```

#### Token Management
- Rotate tokens
- Handle expiration
- Secure storage
- Monitor usage

### 9.4 Rủi Ro & Mitigation

#### 1. Content Quality Issues
**Rủi ro**: AI tạo nội dung kém chất lượng, không phù hợp

**Mitigation**:
- Human review trước khi post
- Quality scoring system
- A/B testing
- Feedback loop

#### 2. Platform Bans
**Rủi ro**: Vi phạm policies → bị ban account

**Mitigation**:
- Tuân thủ tất cả policies
- Không spam
- Đa dạng nội dung
- Backup accounts

#### 3. API Cost Overrun
**Rủi ro**: Chi phí AI APIs vượt budget

**Mitigation**:
- Set spending limits
- Optimize prompts
- Cache results
- Use cheaper models khi có thể

#### 4. Trend Detection False Positives
**Rủi ro**: Follow trends không relevant

**Mitigation**:
- Multi-source validation
- Relevance scoring
- Category filtering
- Manual review cho high-impact posts

---

## 10. TIMELINE & MILESTONES

### Phase 1: Foundation (2-3 tuần)
- [ ] Setup development environment
- [ ] Implement basic API integrations
- [ ] Database schema & setup
- [ ] Basic trend detection
- [ ] Simple content posting

### Phase 2: AI Integration (2-3 tuần)
- [ ] Whisper integration cho transcription
- [ ] Claude API integration
- [ ] Content analysis pipeline
- [ ] Content rewriting system
- [ ] Image generation integration

### Phase 3: Automation (2 tuần)
- [ ] Queue system setup
- [ ] Workflow automation
- [ ] Scheduling system
- [ ] Error handling & retry logic

### Phase 4: Analytics & Optimization (2 tuần)
- [ ] Metrics collection
- [ ] Performance tracking
- [ ] A/B testing system
- [ ] Optimization algorithms

### Phase 5: Testing & Launch (1-2 tuần)
- [ ] End-to-end testing
- [ ] Security audit
- [ ] Performance optimization
- [ ] Soft launch
- [ ] Monitor & iterate

**Tổng thời gian ước tính: 9-12 tuần**

---

## 11. CHI PHÍ ƯỚC TÍNH

### API Costs (Monthly)

#### OpenAI
- **Whisper API**: ~$0.006/minute
  - 100 videos/ngày × 5 phút = 500 phút × $0.006 = $3/ngày = $90/tháng

- **GPT-4 Turbo**: ~$0.01/1K input tokens, $0.03/1K output tokens
  - ~100 analyses/ngày × 4K tokens = $15/ngày = $450/tháng

#### Anthropic Claude
- **Claude Sonnet**: $3/M input tokens, $15/M output tokens
  - Alternative cho GPT-4, similar cost

#### DALL-E / Stable Diffusion
- **DALL-E 3**: $0.040 per image (1024×1024)
  - 50 images/ngày = $2/ngày = $60/tháng

- **Stable Diffusion** (self-hosted): GPU server cost ~$200/tháng

#### Social Media APIs
- **Facebook**: Free (trong rate limits)
- **YouTube**: Free quota 10K units/day
- **TikTok**: Free (cần approval)
- **Zalo**: Free cho basic, paid cho enterprise features

### Infrastructure

- **Server**: $50-200/tháng (Digital Ocean/AWS)
- **Database**: $25-100/tháng
- **Redis**: $10-50/tháng
- **Storage**: $20-100/tháng (videos, images)
- **CDN**: $20-100/tháng

### Total Estimated Cost

- **MVP**: $300-500/tháng
- **Production**: $800-1500/tháng
- **Scale**: $2000-5000/tháng

---

## 12. KẾT LUẬN & KHUYẾN NGHỊ

### Tính Khả Thi
✅ **Khả thi cao** - Tất cả các API và tools cần thiết đều available

### Điểm Mạnh
1. Automation hoàn toàn có thể thực hiện
2. AI tools đủ mạnh để analyze và rewrite content
3. Multi-platform posting được hỗ trợ tốt
4. Trend detection có nhiều data sources

### Thách Thức
1. **Compliance**: Cần cẩn thận với copyright và platform policies
2. **Quality Control**: Cần human review để đảm bảo chất lượng
3. **Cost**: AI APIs có thể tốn kém nếu scale lớn
4. **Platform Restrictions**: TikTok cần audit để post public

### Khuyến Nghị

#### 1. Start Small
- Bắt đầu với 1-2 platforms (Facebook + TikTok)
- Manual review tất cả content initially
- Test với small audience trước

#### 2. Hybrid Approach
- AI generate drafts
- Human review và approve
- Gradually tăng automation khi confident

#### 3. Focus on Quality
- Ưu tiên quality hơn quantity
- Build reputation trước
- Avoid spam behaviors

#### 4. Continuous Learning
- Track metrics carefully
- Learn từ successful posts
- Optimize based on data

#### 5. Legal Compliance
- Consult legal expert
- Clear content policies
- Proper attribution
- Terms of service compliance

### Next Steps

1. **Chọn platform đầu tiên** để pilot (khuyến nghị: Facebook)
2. **Setup development environment**
3. **Implement basic posting workflow**
4. **Test với manual content** trước khi automation
5. **Iterate based on feedback**

---

## PHỤ LỤC

### A. Useful Links

#### Official Documentation
- [Facebook Graph API](https://developers.facebook.com/docs/graph-api/)
- [Zalo Developers](https://developers.zalo.me/docs)
- [YouTube Data API](https://developers.google.com/youtube/v3)
- [TikTok Developers](https://developers.tiktok.com/)

#### AI Services
- [OpenAI API](https://platform.openai.com/docs)
- [Anthropic Claude](https://docs.anthropic.com/)
- [Whisper](https://github.com/openai/whisper)

#### Automation Tools
- [n8n](https://n8n.io/)
- [Bull Queue](https://github.com/OptimalBits/bull)

### B. Code Examples Repository

Sẽ được tạo trong `/examples` directory với:
- Facebook posting example
- Zalo OA messaging example
- YouTube upload example
- TikTok posting example
- Whisper transcription example
- Claude content rewriting example
- Complete workflow example

### C. Sample Workflows

Sẽ được tạo trong `/workflows` directory với:
- Trend detection workflow
- Content analysis workflow
- Content generation workflow
- Multi-platform posting workflow

---

**Document Version**: 1.0
**Last Updated**: 2025-11-11
**Author**: AI Social Post Research Team
