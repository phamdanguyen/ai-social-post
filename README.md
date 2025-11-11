# AI Social Post - Hệ Thống Đăng Bài Tự Động

## Tổng Quan

Hệ thống tự động tìm kiếm nội dung trending/viral trên các mạng xã hội, sử dụng AI để phân tích và viết lại nội dung, sau đó tự động đăng bài lên nhiều kênh mạng xã hội.

## Công Nghệ

- **Ngôn ngữ**: C# (.NET Framework 4.8 hoặc .NET 6+)
- **UI Framework**: WinForms
- **Database**: SQL Server / SQLite
- **APIs**: Facebook Graph API, Zalo API, YouTube Data API, TikTok API
- **AI Services**: OpenAI (Whisper, GPT-4, DALL-E), Anthropic Claude

## Tính Năng Chính

### 1. Phát Hiện Trending
- ✅ Tự động theo dõi trends trên TikTok, YouTube, Google Trends
- ✅ Scoring và ranking trends
- ✅ Filter theo region (Vietnam focus)
- ✅ Real-time updates (15 phút/lần)

### 2. Phân Tích Nội Dung
- ✅ Download video/content từ trending posts
- ✅ Chuyển đổi video thành text (OpenAI Whisper)
- ✅ Phân tích nội dung với AI (Claude/GPT-4)
- ✅ Trích xuất key topics, viral elements

### 3. Tạo Nội Dung Mới
- ✅ Viết lại content với AI
- ✅ Tạo hình ảnh với DALL-E/Stable Diffusion
- ✅ Tạo video clips tự động
- ✅ Tối ưu cho từng platform

### 4. Đăng Bài Tự Động
- ✅ Hỗ trợ 4 platforms: Facebook, Zalo, YouTube, TikTok
- ✅ Lên lịch đăng bài thông minh
- ✅ Tự động thêm hashtags
- ✅ Rate limiting và error handling

### 5. Phân Tích Hiệu Quả
- ✅ Thu thập metrics (likes, shares, comments, views)
- ✅ Theo dõi performance theo thời gian
- ✅ A/B testing
- ✅ Optimization suggestions

## Cấu Trúc Dự Án

```
ai-social-post/
├── docs/                           # Tài liệu
│   ├── SOCIAL_MEDIA_API_RESEARCH.md
│   ├── CSHARP_IMPLEMENTATION.md
│   └── API_INTEGRATION_GUIDE.md
│
├── src/
│   ├── AiSocialPost.sln           # Solution file
│   │
│   ├── AiSocialPost.WinForms/     # WinForms UI Project
│   │   ├── Forms/
│   │   │   ├── MainForm.cs        # Form chính
│   │   │   ├── TrendingForm.cs    # Quản lý trends
│   │   │   ├── ContentForm.cs     # Quản lý content
│   │   │   ├── ScheduleForm.cs    # Lên lịch đăng bài
│   │   │   └── AnalyticsForm.cs   # Phân tích metrics
│   │   ├── Controls/              # Custom controls
│   │   └── Resources/             # Images, icons
│   │
│   ├── AiSocialPost.Core/         # Business Logic
│   │   ├── Models/
│   │   │   ├── TrendingTopic.cs
│   │   │   ├── ContentSource.cs
│   │   │   ├── AnalyzedContent.cs
│   │   │   ├── GeneratedContent.cs
│   │   │   └── PostedContent.cs
│   │   ├── Services/
│   │   │   ├── TrendDetectionService.cs
│   │   │   ├── ContentAnalysisService.cs
│   │   │   ├── ContentGenerationService.cs
│   │   │   ├── SchedulingService.cs
│   │   │   └── AnalyticsService.cs
│   │   └── Interfaces/
│   │
│   ├── AiSocialPost.SocialMedia/  # Social Media Integrations
│   │   ├── Facebook/
│   │   │   ├── FacebookClient.cs
│   │   │   └── FacebookModels.cs
│   │   ├── Zalo/
│   │   │   ├── ZaloClient.cs
│   │   │   └── ZaloModels.cs
│   │   ├── YouTube/
│   │   │   ├── YouTubeClient.cs
│   │   │   └── YouTubeModels.cs
│   │   └── TikTok/
│   │       ├── TikTokClient.cs
│   │       └── TikTokModels.cs
│   │
│   ├── AiSocialPost.AI/           # AI Services
│   │   ├── OpenAI/
│   │   │   ├── WhisperClient.cs
│   │   │   ├── GPT4Client.cs
│   │   │   └── DalleClient.cs
│   │   └── Anthropic/
│   │       └── ClaudeClient.cs
│   │
│   ├── AiSocialPost.Data/         # Data Access Layer
│   │   ├── DbContext/
│   │   ├── Repositories/
│   │   └── Migrations/
│   │
│   └── AiSocialPost.Common/       # Shared Utilities
│       ├── Helpers/
│       ├── Extensions/
│       └── Constants/
│
├── tests/                         # Unit Tests
│   ├── AiSocialPost.Core.Tests/
│   ├── AiSocialPost.SocialMedia.Tests/
│   └── AiSocialPost.AI.Tests/
│
├── examples/                      # Code Examples
│   ├── FacebookPostExample.cs
│   ├── ZaloMessageExample.cs
│   ├── YouTubeUploadExample.cs
│   └── TikTokPostExample.cs
│
└── config/                        # Configuration Files
    ├── appsettings.json
    └── platforms.json
```

## Yêu Cầu Hệ Thống

### Development
- Windows 10/11
- Visual Studio 2022
- .NET Framework 4.8 hoặc .NET 6+
- SQL Server 2019+ hoặc SQLite

### Runtime
- Windows 10/11
- .NET Framework 4.8 hoặc .NET 6+ Runtime
- 4GB RAM minimum (8GB recommended)
- 20GB disk space
- Internet connection

## NuGet Packages Cần Thiết

```xml
<!-- HTTP Client -->
<PackageReference Include="RestSharp" Version="110.2.0" />
<PackageReference Include="Newtonsoft.Json" Version="13.0.3" />

<!-- Facebook SDK -->
<PackageReference Include="Facebook" Version="7.0.6" />

<!-- Google APIs -->
<PackageReference Include="Google.Apis.YouTube.v3" Version="1.63.0.3152" />
<PackageReference Include="Google.Apis.Auth" Version="1.63.0" />

<!-- Database -->
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="7.0.14" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="7.0.14" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="7.0.14" />

<!-- OpenAI -->
<PackageReference Include="OpenAI" Version="1.10.0" />

<!-- Anthropic Claude -->
<PackageReference Include="Anthropic.SDK" Version="1.0.0" />

<!-- Video Processing -->
<PackageReference Include="MediaToolkit" Version="1.1.0.1" />
<PackageReference Include="Xabe.FFmpeg" Version="5.2.6" />

<!-- Job Scheduling -->
<PackageReference Include="Quartz" Version="3.8.0" />
<PackageReference Include="Quartz.Extensions.DependencyInjection" Version="3.8.0" />

<!-- Logging -->
<PackageReference Include="Serilog" Version="3.1.1" />
<PackageReference Include="Serilog.Sinks.File" Version="5.0.0" />

<!-- Configuration -->
<PackageReference Include="Microsoft.Extensions.Configuration" Version="7.0.0" />
<PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="7.0.0" />
```

## Cài Đặt

### 1. Clone Repository
```bash
git clone https://github.com/phamdanguyen/ai-social-post.git
cd ai-social-post
```

### 2. Mở Solution trong Visual Studio
```bash
cd src
start AiSocialPost.sln
```

### 3. Restore NuGet Packages
```
Tools > NuGet Package Manager > Restore NuGet Packages
```

### 4. Cấu Hình Database

**Option 1: SQL Server**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=AiSocialPost;Trusted_Connection=True;"
  }
}
```

**Option 2: SQLite**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=aisocialpost.db"
  }
}
```

### 5. Cấu Hình API Keys

Tạo file `appsettings.json` trong folder config:

```json
{
  "ApiKeys": {
    "OpenAI": {
      "ApiKey": "your-openai-api-key",
      "Organization": "your-org-id"
    },
    "Anthropic": {
      "ApiKey": "your-anthropic-api-key"
    },
    "Facebook": {
      "AppId": "your-fb-app-id",
      "AppSecret": "your-fb-app-secret",
      "PageAccessToken": "your-page-access-token"
    },
    "YouTube": {
      "ClientId": "your-google-client-id",
      "ClientSecret": "your-google-client-secret"
    },
    "TikTok": {
      "ClientKey": "your-tiktok-client-key",
      "ClientSecret": "your-tiktok-client-secret"
    },
    "Zalo": {
      "AppId": "your-zalo-app-id",
      "SecretKey": "your-zalo-secret-key"
    }
  },
  "Settings": {
    "TrendDetectionInterval": 15,
    "MaxTrendsToProcess": 10,
    "EnableAutoPosting": false,
    "RequireManualApproval": true
  }
}
```

### 6. Chạy Database Migrations
```bash
dotnet ef database update --project src/AiSocialPost.Data
```

### 7. Build và Run
```
F5 hoặc Ctrl+F5
```

## Hướng Dẫn Sử Dụng

### 1. Khởi Động Lần Đầu

1. Mở ứng dụng
2. Vào **Settings** → **API Configuration**
3. Nhập API keys cho các services
4. Click **Test Connections** để verify
5. Click **Save**

### 2. Kết Nối Social Media Accounts

1. Vào **Social Media** → **Connect Accounts**
2. Chọn platform (Facebook/Zalo/YouTube/TikTok)
3. Click **Authorize** và follow OAuth flow
4. Verify connection thành công

### 3. Bắt Đầu Theo Dõi Trends

1. Vào **Trending** tab
2. Click **Start Monitoring**
3. Xem danh sách trends được phát hiện
4. Filter theo platform, region, category

### 4. Phân Tích và Tạo Content

1. Trong **Trending** tab, chọn một trend
2. Click **Analyze Content**
3. Xem kết quả phân tích
4. Click **Generate Content**
5. Review generated content cho từng platform
6. Edit nếu cần thiết

### 5. Lên Lịch Đăng Bài

1. Vào **Schedule** tab
2. Chọn content đã generate
3. Chọn platforms để đăng
4. Chọn thời gian đăng (hoặc dùng AI suggest)
5. Click **Schedule**

### 6. Theo Dõi Performance

1. Vào **Analytics** tab
2. Xem overview metrics
3. Click vào specific posts để xem detail
4. Export reports nếu cần

## Workflow Tự Động

Khi enable **Auto Mode**:

```
[Mỗi 15 phút]
1. Phát hiện trends mới
   ↓
2. Analyze top content từ trends
   ↓
3. Generate content cho tất cả platforms
   ↓
4. [Nếu RequireManualApproval = false]
   Auto schedule posts
   ↓
5. Post vào thời điểm đã schedule
   ↓
6. Collect metrics mỗi giờ trong 24h đầu
```

## Tính Năng Nâng Cao

### 1. Custom Templates
- Tạo templates cho từng loại content
- Variables: {topic}, {hashtags}, {date}
- Platform-specific templates

### 2. A/B Testing
- Test multiple versions của cùng content
- Auto-select winner dựa trên metrics
- Learn và optimize theo thời gian

### 3. Audience Targeting
- Define target audience cho từng platform
- Optimize content cho audience
- Best time to post suggestions

### 4. Content Calendar
- Visual calendar view
- Drag-drop scheduling
- Bulk operations
- Export/import

### 5. Team Collaboration
- Multiple user accounts
- Role-based permissions
- Approval workflows
- Activity logs

## Troubleshooting

### API Connection Issues
1. Check internet connection
2. Verify API keys
3. Check rate limits
4. Review error logs in `logs/` folder

### Content Generation Fails
1. Check OpenAI/Claude API quota
2. Verify API key validity
3. Check content length limits
4. Review error messages

### Posting Fails
1. Verify platform authentication
2. Check content compliance với platform policies
3. Verify rate limits
4. Check file size limits (cho videos)

## Chi Phí Vận Hành

### Ước Tính Monthly Costs

#### AI APIs
- OpenAI Whisper: ~$90/tháng
- GPT-4 Turbo: ~$450/tháng
- DALL-E: ~$60/tháng
- **Total AI**: ~$600/tháng

#### Infrastructure
- SQL Server (optional): $0-50/tháng
- Storage: $10-50/tháng
- **Total Infra**: ~$10-100/tháng

**Total Estimated**: $610-700/tháng cho moderate usage

### Cost Optimization Tips
1. Sử dụng GPT-3.5 thay vì GPT-4 cho simple tasks
2. Cache transcript results
3. Batch processing
4. Set daily spending limits

## Roadmap

### Version 1.0 (MVP) - Q1 2025
- [x] Research APIs
- [ ] Basic UI với WinForms
- [ ] Facebook integration
- [ ] Simple trending detection
- [ ] Manual posting

### Version 1.5 - Q2 2025
- [ ] TikTok integration
- [ ] AI content generation
- [ ] Auto scheduling
- [ ] Basic analytics

### Version 2.0 - Q3 2025
- [ ] YouTube integration
- [ ] Zalo integration
- [ ] Advanced analytics
- [ ] A/B testing
- [ ] Team collaboration

### Version 2.5 - Q4 2025
- [ ] Mobile app companion
- [ ] Advanced AI features
- [ ] Custom video generation
- [ ] White label option

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

## Acknowledgments

- OpenAI cho Whisper, GPT-4, DALL-E
- Anthropic cho Claude
- Facebook, YouTube, TikTok, Zalo cho APIs
- Tất cả contributors và testers

---

**Note**: Đây là research project. Vui lòng tuân thủ tất cả terms of service của các platforms và respect copyright laws.
