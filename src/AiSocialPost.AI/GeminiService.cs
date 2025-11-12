using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AiSocialPost.AI
{
    public class GeminiService
    {
        private readonly GeminiRateLimiter _rateLimiter;
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash-exp:generateContent";

        public GeminiService(GeminiRateLimiter rateLimiter, string apiKey = "")
        {
            _rateLimiter = rateLimiter;
            _apiKey = apiKey;
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(60);
        }

        public async Task<string> GenerateContentAsync(string prompt)
        {
            await _rateLimiter.WaitIfNeededAsync();

            if (string.IsNullOrEmpty(_apiKey))
            {
                return "[ERROR] Gemini API key chưa được cấu hình. Vui lòng vào Settings để thêm API key.";
            }

            try
            {
                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.7,
                        topP = 0.95,
                        maxOutputTokens = 2048
                    }
                };

                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"{BaseUrl}?key={_apiKey}";
                var response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return $"[ERROR] Gemini API Error: {response.StatusCode}\n{errorContent}";
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                var result = JObject.Parse(responseBody);

                var text = result["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();
                return text ?? "[ERROR] Không thể parse response từ Gemini";
            }
            catch (HttpRequestException ex)
            {
                return $"[ERROR] Network error: {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"[ERROR] {ex.Message}";
            }
        }

        public async Task<string> AnalyzeTrendAsync(string topic)
        {
            var prompt = $@"Bạn là chuyên gia phân tích trends trên mạng xã hội Việt Nam.

Phân tích topic trending sau: ""{topic}""

Cung cấp thông tin:
1. Tại sao topic này đang trending?
2. Những điểm nổi bật, viral elements
3. Target audience chính
4. Tone và style phù hợp để viết content
5. 5-10 hashtags liên quan

Trả lời bằng tiếng Việt, ngắn gọn, dễ hiểu.";

            return await GenerateContentAsync(prompt);
        }

        public async Task<string> GeneratePostFromTrendAsync(string trendTopic, string platform)
        {
            var prompt = $@"Bạn là content creator chuyên nghiệp cho {platform}.

Tạo 1 bài viết về topic trending: ""{trendTopic}""

Yêu cầu:
- Phù hợp với phong cách {platform}
- Độ dài: {GetRecommendedLength(platform)}
- Tone: Thân thiện, hấp dẫn
- Ngôn ngữ: Tiếng Việt
- Hook mạnh ở đầu
- CTA ở cuối (nếu phù hợp)

CHỈ trả về nội dung bài viết, KHÔNG thêm giải thích.";

            return await GenerateContentAsync(prompt);
        }

        public async Task<string> GenerateHashtagsAsync(string content)
        {
            var prompt = $@"Dựa vào nội dung bài viết sau, đề xuất 5-10 hashtags phù hợp.

Nội dung:
""{content}""

CHỈ trả về danh sách hashtags, mỗi hashtag cách nhau bởi dấu cách, ví dụ: #AI #Technology #Vietnam
KHÔNG thêm giải thích.";

            return await GenerateContentAsync(prompt);
        }

        public async Task<string> GenerateReplyAsync(string commentText)
        {
            var prompt = $@"Bạn là người quản lý fanpage thân thiện.

User comment: ""{commentText}""

Tạo 1 câu trả lời ngắn gọn, thân thiện, tự nhiên.
CHỈ trả về câu trả lời, KHÔNG thêm giải thích.
Độ dài: Tối đa 2 câu.";

            return await GenerateContentAsync(prompt);
        }

        public async Task<List<string>> DetectTrendsAsync(string region = "Vietnam")
        {
            var prompt = $@"Bạn là chuyên gia phân tích trends mạng xã hội tại {region}.

Liệt kê 10 topics đang trending HIỆN TẠI (hôm nay) trên các nền tảng: Facebook, TikTok, YouTube, Twitter/X.

CHỈ trả về danh sách topics, mỗi topic 1 dòng, KHÔNG thêm giải thích hay số thứ tự.
Ví dụ:
AI và tương lai công nghệ
Công thức nấu ăn 5 phút
Review sản phẩm mới";

            var response = await GenerateContentAsync(prompt);

            if (response.StartsWith("[ERROR]"))
            {
                return new List<string> { "Sample Trend 1", "Sample Trend 2", "Sample Trend 3" };
            }

            var lines = response.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                                .Select(l => l.Trim())
                                .Where(l => !string.IsNullOrEmpty(l) && !l.StartsWith("["))
                                .Take(10)
                                .ToList();

            return lines.Count > 0 ? lines : new List<string> { "Sample Trend 1", "Sample Trend 2", "Sample Trend 3" };
        }

        private string GetRecommendedLength(string platform)
        {
            return platform.ToLower() switch
            {
                "facebook" => "100-300 từ",
                "tiktok" => "50-100 từ (ngắn gọn)",
                "youtube" => "200-500 từ (mô tả video)",
                "twitter" => "50-280 ký tự",
                "zalo" => "100-200 từ",
                _ => "100-300 từ"
            };
        }
    }
}
