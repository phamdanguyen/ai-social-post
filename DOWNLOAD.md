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
3. Nhập API keys (bắt buộc để sử dụng tính năng):
   - **Gemini API Key** (bắt buộc cho AI) - Đã có sẵn: `AIzaSyAQOaTnPMPc9YEdEViXBjUKX9s0x8VS8_w`
   - **FB Page ID** (bắt buộc cho Facebook posting) - VD: `123456789012345`
   - **FB Page Token** (bắt buộc cho Facebook posting) - Lấy từ [Graph API Explorer](https://developers.facebook.com/tools/explorer/)
   - TikTok Client Key (chưa implement)
   - YouTube Client ID (chưa implement)
   - Zalo App ID (chưa implement)
   - Twitter API Key (chưa implement)
   - Banana API Key (chưa implement)
4. Click **Lưu** và khởi động lại app

#### Cách lấy Facebook Page Access Token:
1. Vào https://developers.facebook.com/tools/explorer/
2. Chọn "Get Token" → "Get Page Access Token"
3. Chọn Page của bạn
4. Copy token và paste vào Settings
5. **Lưu ý**: Token này có thời hạn, cần renew định kỳ

### Bước 2: Tìm Trends

1. Click nút **Tìm Trends**
2. Xem danh sách trends (sử dụng Gemini AI phân tích trends thực tế tại Vietnam)
3. Click chọn 1 trend

### Bước 3: Tạo Content

1. Click **Tạo Content**
2. Xem content AI tạo (sử dụng Gemini AI tạo nội dung chất lượng cao)
3. Edit nếu muốn

### Bước 4: Đăng Bài

1. Chọn platforms (Facebook đã hoạt động, còn lại là placeholder)
2. Click **Đăng Ngay** để đăng lên Facebook
3. Chờ đăng thành công (Facebook posting thật, các platform khác chưa implement)

---

## ⚠️ Lưu Ý - MVP Version

**Phiên bản hiện tại** có:
- ✅ UI hoàn chỉnh
- ✅ SQLite database
- ✅ Rate limiting (10 req/min cho Gemini)
- ✅ Gemini AI integration (trend detection + content generation)
- ✅ Facebook posting (thật, cần Page Access Token)
- ✅ Facebook comment monitoring với AI replies
- ✅ Lead detection từ keywords (giá, mua, order, ship, v.v.)
- ❌ OAuth2 flow (dùng manual token thay thế)
- ❌ TikTok, YouTube, Zalo, Twitter posting (chưa implement)
- ❌ Scheduling (chỉ đăng ngay)

**Facebook đã hoạt động thật!** Các platform khác vẫn là placeholder.

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
- [x] **Week 2**: Gemini API integration ✅ DONE
- [x] **Week 3**: Facebook posting & comment monitoring ✅ DONE
- [ ] **Week 4-5**: TikTok, YouTube posting implementations
- [ ] **Week 6**: Zalo, Twitter posting implementations
- [ ] **Week 7**: Scheduling system
- [ ] **Week 8**: Testing & polish

**Hiện tại đã có Gemini AI và Facebook posting thật!**
