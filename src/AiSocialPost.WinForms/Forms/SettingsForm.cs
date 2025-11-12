using Microsoft.Extensions.Configuration;

namespace AiSocialPost.WinForms.Forms
{
    public partial class SettingsForm : Form
    {
        private readonly IConfiguration _configuration;

        public SettingsForm(IConfiguration configuration)
        {
            _configuration = configuration;
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            txtGeminiApiKey.Text = _configuration["Google:ApiKey"] ?? "";
            txtFacebookAppId.Text = _configuration["Facebook:AppId"] ?? "";
            txtFacebookAppSecret.Text = _configuration["Facebook:AppSecret"] ?? "";
            txtTikTokClientKey.Text = _configuration["TikTok:ClientKey"] ?? "";
            txtYouTubeClientId.Text = _configuration["Google:YouTubeClientId"] ?? "";
            txtZaloAppId.Text = _configuration["Zalo:AppId"] ?? "";
            txtTwitterApiKey.Text = _configuration["Twitter:ApiKey"] ?? "";
            txtBananaApiKey.Text = _configuration["Banana:ApiKey"] ?? "";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
                var json = System.IO.File.ReadAllText(configPath);
                var config = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

                // Update values
                if (config == null) config = new System.Dynamic.ExpandoObject();

                config.Google = config.Google ?? new System.Dynamic.ExpandoObject();
                config.Google.ApiKey = txtGeminiApiKey.Text;
                config.Google.YouTubeClientId = txtYouTubeClientId.Text;

                config.Facebook = config.Facebook ?? new System.Dynamic.ExpandoObject();
                config.Facebook.AppId = txtFacebookAppId.Text;
                config.Facebook.AppSecret = txtFacebookAppSecret.Text;

                config.TikTok = config.TikTok ?? new System.Dynamic.ExpandoObject();
                config.TikTok.ClientKey = txtTikTokClientKey.Text;

                config.Zalo = config.Zalo ?? new System.Dynamic.ExpandoObject();
                config.Zalo.AppId = txtZaloAppId.Text;

                config.Twitter = config.Twitter ?? new System.Dynamic.ExpandoObject();
                config.Twitter.ApiKey = txtTwitterApiKey.Text;

                config.Banana = config.Banana ?? new System.Dynamic.ExpandoObject();
                config.Banana.ApiKey = txtBananaApiKey.Text;

                // Save to file
                var updatedJson = Newtonsoft.Json.JsonConvert.SerializeObject(config, Newtonsoft.Json.Formatting.Indented);
                System.IO.File.WriteAllText(configPath, updatedJson);

                MessageBox.Show("Đã lưu settings thành công! Khởi động lại app để áp dụng.",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu settings: {ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnTestConnection_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng test connection đang được phát triển!",
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
