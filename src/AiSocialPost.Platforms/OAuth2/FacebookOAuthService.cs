using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace AiSocialPost.Platforms.OAuth2
{
    public class FacebookOAuthService
    {
        private readonly string _appId;
        private readonly string _appSecret;
        private const string RedirectUri = "http://localhost:8080/callback";
        private const string AuthUrl = "https://www.facebook.com/v22.0/dialog/oauth";
        private const string TokenUrl = "https://graph.facebook.com/v22.0/oauth/access_token";

        public FacebookOAuthService(string appId, string appSecret)
        {
            _appId = appId;
            _appSecret = appSecret;
        }

        public string GetAuthorizationUrl()
        {
            var scopes = new[] { "pages_manage_posts", "pages_read_engagement", "pages_read_user_content" };
            var state = Guid.NewGuid().ToString();

            var url = $"{AuthUrl}?" +
                     $"client_id={_appId}" +
                     $"&redirect_uri={Uri.EscapeDataString(RedirectUri)}" +
                     $"&scope={string.Join(",", scopes)}" +
                     $"&response_type=code" +
                     $"&state={state}";

            return url;
        }

        public async Task<string> AuthenticateAsync()
        {
            var authUrl = GetAuthorizationUrl();

            // Open browser
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = authUrl,
                UseShellExecute = true
            });

            // Start local HTTP server to receive callback
            var code = await ListenForCallbackAsync();

            if (string.IsNullOrEmpty(code))
            {
                throw new Exception("Authentication failed - No authorization code received");
            }

            // Exchange code for access token
            var accessToken = await ExchangeCodeForTokenAsync(code);

            return accessToken;
        }

        private async Task<string> ListenForCallbackAsync()
        {
            var listener = new HttpListener();
            listener.Prefixes.Add(RedirectUri + "/");
            listener.Start();

            try
            {
                var context = await listener.GetContextAsync();
                var request = context.Request;
                var response = context.Response;

                // Get authorization code from query string
                var code = request.QueryString["code"];

                // Send response to browser
                var responseString = code != null
                    ? "<html><body><h1>✅ Đăng nhập thành công!</h1><p>Bạn có thể đóng tab này.</p></body></html>"
                    : "<html><body><h1>❌ Đăng nhập thất bại!</h1></body></html>";

                var buffer = Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                response.OutputStream.Close();

                return code ?? "";
            }
            finally
            {
                listener.Stop();
            }
        }

        private async Task<string> ExchangeCodeForTokenAsync(string code)
        {
            using var httpClient = new HttpClient();

            var url = $"{TokenUrl}?" +
                     $"client_id={_appId}" +
                     $"&client_secret={_appSecret}" +
                     $"&redirect_uri={Uri.EscapeDataString(RedirectUri)}" +
                     $"&code={code}";

            var response = await httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to exchange code for token: {content}");
            }

            var json = JObject.Parse(content);
            var accessToken = json["access_token"]?.ToString();

            if (string.IsNullOrEmpty(accessToken))
            {
                throw new Exception("No access token in response");
            }

            // Get long-lived token
            return await GetLongLivedTokenAsync(accessToken);
        }

        private async Task<string> GetLongLivedTokenAsync(string shortLivedToken)
        {
            using var httpClient = new HttpClient();

            var url = $"https://graph.facebook.com/v22.0/oauth/access_token?" +
                     $"grant_type=fb_exchange_token" +
                     $"&client_id={_appId}" +
                     $"&client_secret={_appSecret}" +
                     $"&fb_exchange_token={shortLivedToken}";

            var response = await httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                // If long-lived token fails, return short-lived token
                return shortLivedToken;
            }

            var json = JObject.Parse(content);
            return json["access_token"]?.ToString() ?? shortLivedToken;
        }

        public async Task<List<FacebookPage>> GetPagesAsync(string accessToken)
        {
            using var httpClient = new HttpClient();

            var url = $"https://graph.facebook.com/v22.0/me/accounts?access_token={accessToken}";
            var response = await httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get pages: {content}");
            }

            var json = JObject.Parse(content);
            var pages = new List<FacebookPage>();

            foreach (var page in json["data"] ?? new JArray())
            {
                pages.Add(new FacebookPage
                {
                    Id = page["id"]?.ToString() ?? "",
                    Name = page["name"]?.ToString() ?? "",
                    AccessToken = page["access_token"]?.ToString() ?? ""
                });
            }

            return pages;
        }
    }

    public class FacebookPage
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string AccessToken { get; set; } = "";
    }
}
