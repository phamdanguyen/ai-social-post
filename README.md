# AI Social Post - Tool Đăng Bài Tự Động

## Tổng Quan

Tool đơn giản giúp tự động tìm trend, dùng AI viết lại content, và đăng bài lên các mạng xã hội. Tất cả chạy trên 1 app WinForms duy nhất.

## Công Nghệ

- **Ngôn ngữ**: C# (.NET 6+)
- **UI Framework**: WinForms (Desktop app)
- **Database**: SQLite (đơn giản, không cần server)
- **Platforms**: Facebook, TikTok, YouTube, Zalo, Twitter/X
- **AI**: Google Gemini (chính), Banana API (ảnh), Veo 3 (video)
- **Authentication**: OAuth2 (⭐ BẮT BUỘC - Đọc [OAuth2 Analysis](docs/OAUTH2_ANALYSIS.md))

## 4 Chức Năng Chính

### 1. Tự Động Tìm Trend
- AI phát hiện topic đang hot (dùng Gemini + Google Search)
- Hỗ trợ 5 platforms: Facebook, TikTok, YouTube, Zalo, Twitter/X
- Cập nhật mỗi tuần (hoặc manual khi cần)
- Hiển thị trend score và độ hot

### 2. AI Viết Lại Content
- Dùng Google Gemini phân tích nội dung viral
- Tự động viết lại phù hợp với từng platform
- Tạo hashtags và caption
- Tạo ảnh (Banana API) và video (Veo 3) nếu cần

### 3. Đăng Bài Tự Động
- Post lên 5 platforms: Facebook, TikTok, YouTube, Zalo, X
- Tự động chọn thời gian tốt nhất để đăng
- Hoặc lên lịch đăng theo ý muốn
- Xử lý lỗi và retry tự động

### 4. AI Trả Lời Comment
- Tự động theo dõi comments trên bài đăng
- AI trả lời các câu hỏi đơn giản
- Phát hiện ý định mua hàng ("giá bao nhiêu?")
- Tự động like comments

## Cấu Trúc Dự Án

```
ai-social-post/
├── docs/                                    # Tài liệu
│   ├── SYSTEM_ARCHITECTURE_AND_USER_STORIES.md  # ⭐ ĐỌC ĐẦU TIÊN
│   ├── GEMINI_FIRST_STRATEGY.md             # AI strategy (đơn giản)
│   ├── OAUTH2_ANALYSIS.md                   # OAuth2 (bắt buộc)
│   ├── SOCIAL_MEDIA_API_RESEARCH.md         # API details
│   ├── SCHEDULING_AND_TIMING_STRATEGY.md    # Best times to post
│   └── ENGAGEMENT_AUTOMATION.md             # AI reply comments
│
├── src/
│   ├── AiSocialPost.sln           # Solution file
│   │
│   ├── AiSocialPost.WinForms/     # ⭐ MAIN APP
│   │   ├── Forms/
│   │   │   ├── MainForm.cs        # Form chính (tất cả trong 1)
│   │   │   └── SettingsForm.cs    # Cấu hình API keys
│   │   └── Resources/             # Images, icons
│   │
│   ├── AiSocialPost.Core/         # Business Logic
│   │   ├── Models/
│   │   │   ├── TrendingTopic.cs
│   │   │   ├── GeneratedContent.cs
│   │   │   └── Comment.cs
│   │   └── Services/
│   │       ├── TrendService.cs         # Tìm trends
│   │       ├── ContentService.cs       # Tạo content
│   │       ├── PostingService.cs       # Đăng bài
│   │       └── CommentService.cs       # Reply comments
│   │
│   ├── AiSocialPost.Platforms/    # Platform Clients
│   │   ├── FacebookClient.cs
│   │   ├── TikTokClient.cs
│   │   ├── YouTubeClient.cs
│   │   ├── ZaloClient.cs
│   │   └── TwitterClient.cs
│   │
│   ├── AiSocialPost.AI/           # AI Services
│   │   ├── GeminiService.cs       # Google Gemini (chính)
│   │   ├── BananaService.cs       # Tạo ảnh
│   │   └── VeoService.cs          # Tạo video
│   │
│   └── AiSocialPost.Data/         # SQLite Database
│       ├── AppDbContext.cs
│       └── aisocialpost.db        # SQLite file
│
└── config/
    └── appsettings.json           # API keys và settings
```

## Yêu Cầu Hệ Thống

- Windows 10/11
- Visual Studio 2022
- .NET 6+
- 4GB RAM
- Internet connection
- SQLite (tự động tạo, không cần cài gì thêm)

## NuGet Packages Cần Thiết

```xml
<!-- HTTP & JSON -->
<PackageReference Include="Newtonsoft.Json" Version="13.0.3" />

<!-- Database (SQLite) -->
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="7.0.14" />

<!-- Google APIs (YouTube, Gemini) -->
<PackageReference Include="Google.Apis.YouTube.v3" Version="1.63.0.3152" />
<PackageReference Include="Google.Apis.Auth" Version="1.63.0" />
<PackageReference Include="Google.GenerativeAI" Version="1.0.0" />

<!-- Configuration -->
<PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="7.0.0" />
```

## Cài Đặt

### 1. Clone và Mở Project
```bash
git clone https://github.com/phamdanguyen/ai-social-post.git
cd ai-social-post/src
start AiSocialPost.sln
```

### 2. Cấu Hình API Keys

Tạo file `config/appsettings.json`:

```json
{
  "Google": {
    "ApiKey": "your-gemini-api-key",
    "YouTubeClientId": "your-youtube-client-id",
    "YouTubeClientSecret": "your-youtube-client-secret"
  },
  "Facebook": {
    "AppId": "your-fb-app-id",
    "AppSecret": "your-fb-app-secret"
  },
  "TikTok": {
    "ClientKey": "your-tiktok-client-key",
    "ClientSecret": "your-tiktok-client-secret"
  },
  "Zalo": {
    "AppId": "your-zalo-app-id",
    "SecretKey": "your-zalo-secret-key"
  },
  "Twitter": {
    "ApiKey": "your-twitter-api-key",
    "ApiSecret": "your-twitter-api-secret"
  },
  "Banana": {
    "ApiKey": "your-banana-api-key"
  }
}
```

### 3. Build và Run
```
F5 trong Visual Studio
```

Database (SQLite) sẽ tự động tạo khi chạy lần đầu.

## Hướng Dẫn Sử Dụng

### 1. Kết Nối Accounts

1. Mở app, click **Settings**
2. Nhập API keys đã có
3. Click **Connect** cho từng platform (Facebook, TikTok, YouTube, Zalo, Twitter)
4. Follow OAuth flow

### 2. Tìm Trends

1. Click **Tìm Trends**
2. Xem danh sách trends AI phát hiện
3. Chọn trend muốn sử dụng

### 3. Tạo Content

1. Click **Tạo Content** từ trend đã chọn
2. AI sẽ phân tích và viết lại
3. Review và edit nếu cần
4. Chọn platforms để đăng (có thể chọn nhiều)

### 4. Đăng Bài

1. Click **Đăng Ngay** hoặc **Lên Lịch**
2. Nếu lên lịch, chọn thời gian (hoặc để AI chọn thời gian tốt nhất)
3. Chờ đăng thành công

### 5. AI Trả Lời Comments

- Tự động chạy background
- Xem comments đã reply trong tab **Comments**
- Có thể tắt/bật auto-reply trong Settings

## Workflow Đơn Giản

```
1. AI tìm trends (1 lần/tuần hoặc manual)
   ↓
2. Bạn chọn trend muốn dùng
   ↓
3. AI viết lại content
   ↓
4. Bạn review và edit (nếu muốn)
   ↓
5. Chọn platforms và đăng
   ↓
6. AI tự động reply comments
```

## Chi Phí Ước Tính

### API Costs (Monthly - Usage Vừa Phải)

- **Google Gemini**: ~$150/tháng (trend detection + content generation)
- **Banana API**: ~$100/tháng (tạo ảnh)
- **Veo 3**: ~$200/tháng (tạo video - optional)

**Total**: ~$450/tháng (hoặc $250/tháng nếu không dùng video)

### Gemini Free Tier (Alternative)
Nếu dùng free tier:
- **15 requests/minute** (RPM)
- **1 million tokens/minute** (TPM)
- **1,500 requests/day** (RPD)

**⚠️ QUAN TRỌNG**: App có **rate limiting** để tôn trọng limits của Gemini:
- Tự động queue requests
- Retry với exponential backoff khi hit limit
- Không vượt quá 10 requests/minute (safety margin)

### Lưu Ý
- Chi phí phụ thuộc vào usage
- Có thể bỏ Veo 3 nếu không cần video
- Gemini rẻ hơn nhiều so với GPT-4
- **Luôn tôn trọng rate limits** để tránh bị block

## Roadmap

### Version 1.0 (MVP) - 8 tuần
- [x] Research APIs & Architecture
- [ ] WinForms UI (MainForm + SettingsForm)
- [ ] OAuth2 cho 5 platforms
- [ ] Gemini integration (trend + content)
- [ ] Đăng bài cơ bản
- [ ] AI reply comments

**Chỉ làm MVP thôi. Không làm thêm tính năng phức tạp.**

## Đóng Góp

Mọi đóng góp đều được hoan nghênh! Vui lòng:

1. Fork repository
2. Tạo feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Mở Pull Request

## License

[MIT License](LICENSE)

## Liên Hệ

- GitHub: [@phamdanguyen](https://github.com/phamdanguyen)
- Email: your-email@example.com

## 📚 Tài Liệu

### Đọc Theo Thứ Tự

1. **[README.md](README.md)** (file này) - Tổng quan
2. **[SYSTEM_ARCHITECTURE_AND_USER_STORIES.md](docs/SYSTEM_ARCHITECTURE_AND_USER_STORIES.md)** - User stories & kiến trúc đơn giản
3. **[GEMINI_FIRST_STRATEGY.md](docs/GEMINI_FIRST_STRATEGY.md)** - AI strategy (Gemini + Banana + Veo)
4. **[OAUTH2_ANALYSIS.md](docs/OAUTH2_ANALYSIS.md)** - OAuth2 (bắt buộc đọc)
5. **[SOCIAL_MEDIA_API_RESEARCH.md](docs/SOCIAL_MEDIA_API_RESEARCH.md)** - API details
6. **[SCHEDULING_AND_TIMING_STRATEGY.md](docs/SCHEDULING_AND_TIMING_STRATEGY.md)** - Best times to post
7. **[ENGAGEMENT_AUTOMATION.md](docs/ENGAGEMENT_AUTOMATION.md)** - AI reply comments

**Lưu ý**: Các tài liệu trên có thể chứa tính năng nâng cao, nhưng chỉ implement **4 chức năng cốt lõi** như đã mô tả trong README này.

## Credits

- Google Gemini cho AI
- Banana API cho image generation
- Facebook, YouTube, TikTok, Zalo, Twitter cho APIs

---

**Note**: Tool đơn giản cho personal use. Vui lòng tuân thủ terms of service của platforms.
