# 📥 Download AI Social Post

## Cách Tải App

### Option 1: Download từ GitHub Actions (Recommended)

1. Vào trang **Actions**: https://github.com/phamdanguyen/ai-social-post/actions

2. Click vào workflow run mới nhất (build thành công - dấu ✅ xanh)

3. Scroll xuống phần **Artifacts**

4. Click download **AiSocialPost-Windows-x64**

5. Giải nén file ZIP

6. Chạy `AiSocialPost.exe`

### Option 2: Build Từ Source Code

```bash
git clone https://github.com/phamdanguyen/ai-social-post.git
cd ai-social-post/src
dotnet publish AiSocialPost.WinForms/AiSocialPost.WinForms.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
```

File .exe sẽ nằm trong: `src/AiSocialPost.WinForms/bin/Release/net6.0/win-x64/publish/AiSocialPost.exe`

---

## 🚀 Hướng Dẫn Sử Dụng (MVP Version)

### Bước 1: Cấu Hình API Keys

1. Chạy `AiSocialPost.exe`
2. Click nút **Settings**
3. Nhập API keys (tùy chọn):
   - **Gemini API Key** (chính)
   - Facebook App ID/Secret
   - TikTok Client Key
   - YouTube Client ID
   - Zalo App ID
   - Twitter API Key
   - Banana API Key
4. Click **Lưu**

### Bước 2: Tìm Trends

1. Click nút **Tìm Trends**
2. Xem danh sách trends (hiện tại là sample data)
3. Click chọn 1 trend

### Bước 3: Tạo Content

1. Click **Tạo Content**
2. Xem content AI tạo (hiện tại là placeholder)
3. Edit nếu muốn

### Bước 4: Đăng Bài

1. Chọn platforms (Facebook, TikTok, YouTube, Zalo, Twitter)
2. Click **Đăng Ngay** hoặc **Lên Lịch**
3. Chờ đăng thành công (hiện tại là placeholder)

---

## ⚠️ Lưu Ý - MVP Version

**Phiên bản hiện tại (Week 1 Foundation)** chỉ có:
- ✅ UI hoàn chỉnh
- ✅ SQLite database
- ✅ Rate limiting (10 req/min)
- ❌ Chưa có OAuth2 (Week 2-3)
- ❌ Chưa integrate Gemini API thật (Week 4-5)
- ❌ Chưa đăng bài thật lên platforms (Week 6-7)

**Tất cả đều là PLACEHOLDER** - chỉ để test UI và workflow!

---

## 🔧 Yêu Cầu Hệ Thống

- **OS**: Windows 10/11
- **RAM**: 4GB
- **.NET**: Không cần cài (self-contained)
- **Disk**: ~50MB

---

## 🐛 Báo Lỗi

Nếu gặp lỗi, tạo issue tại: https://github.com/phamdanguyen/ai-social-post/issues

---

## 📅 Lộ Trình Phát Triển

- [x] **Week 1**: Foundation (UI, Database, Services) ✅ DONE
- [ ] **Week 2-3**: OAuth2 cho 5 platforms
- [ ] **Week 4-5**: Gemini API integration
- [ ] **Week 6-7**: Platform posting implementations
- [ ] **Week 8**: Testing & polish

**Check lại sau 2 tuần để có phiên bản với OAuth2!**
