using Newtonsoft.Json.Linq;

namespace AiSocialPost.Platforms.Clients
{
    public class FacebookClient
    {
        private readonly string _pageId;
        private readonly string _accessToken;
        private readonly HttpClient _httpClient;

        public FacebookClient(string pageId, string accessToken)
        {
            _pageId = pageId;
            _accessToken = accessToken;
            _httpClient = new HttpClient();
        }

        public async Task<string> PostAsync(string message, string? imageUrl = null)
        {
            var url = $"https://graph.facebook.com/v22.0/{_pageId}/feed";

            var parameters = new Dictionary<string, string>
            {
                { "message", message },
                { "access_token", _accessToken }
            };

            if (!string.IsNullOrEmpty(imageUrl))
            {
                parameters.Add("link", imageUrl);
            }

            var content = new FormUrlEncodedContent(parameters);
            var response = await _httpClient.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Facebook post failed: {responseContent}");
            }

            var json = JObject.Parse(responseContent);
            return json["id"]?.ToString() ?? "";
        }

        public async Task<List<FacebookComment>> GetCommentsAsync(string postId)
        {
            var url = $"https://graph.facebook.com/v22.0/{postId}/comments?access_token={_accessToken}";

            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get comments: {content}");
            }

            var json = JObject.Parse(content);
            var comments = new List<FacebookComment>();

            foreach (var comment in json["data"] ?? new JArray())
            {
                comments.Add(new FacebookComment
                {
                    Id = comment["id"]?.ToString() ?? "",
                    Message = comment["message"]?.ToString() ?? "",
                    From = comment["from"]?["name"]?.ToString() ?? "",
                    FromId = comment["from"]?["id"]?.ToString() ?? "",
                    CreatedTime = DateTime.Parse(comment["created_time"]?.ToString() ?? DateTime.Now.ToString())
                });
            }

            return comments;
        }

        public async Task<string> ReplyToCommentAsync(string commentId, string message)
        {
            var url = $"https://graph.facebook.com/v22.0/{commentId}/comments";

            var parameters = new Dictionary<string, string>
            {
                { "message", message },
                { "access_token", _accessToken }
            };

            var content = new FormUrlEncodedContent(parameters);
            var response = await _httpClient.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to reply to comment: {responseContent}");
            }

            var json = JObject.Parse(responseContent);
            return json["id"]?.ToString() ?? "";
        }

        public async Task<bool> LikeCommentAsync(string commentId)
        {
            var url = $"https://graph.facebook.com/v22.0/{commentId}/likes?access_token={_accessToken}";

            var response = await _httpClient.PostAsync(url, null);
            return response.IsSuccessStatusCode;
        }
    }

    public class FacebookComment
    {
        public string Id { get; set; } = "";
        public string Message { get; set; } = "";
        public string From { get; set; } = "";
        public string FromId { get; set; } = "";
        public DateTime CreatedTime { get; set; }
    }
}
