using AiSocialPost.Core.Services;
using AiSocialPost.Core.Models;
using AiSocialPost.Data;

namespace AiSocialPost.WinForms.Forms
{
    public partial class MainForm : Form
    {
        private readonly ITrendService _trendService;
        private readonly IContentService _contentService;
        private readonly IPostingService _postingService;
        private readonly ICommentService _commentService;
        private readonly AppDbContext _dbContext;

        private List<TrendingTopic> _currentTrends = new();
        private TrendingTopic? _selectedTrend = null;
        private GeneratedContent? _generatedContent = null;

        public MainForm(
            ITrendService trendService,
            IContentService contentService,
            IPostingService postingService,
            ICommentService commentService,
            AppDbContext dbContext)
        {
            _trendService = trendService;
            _contentService = contentService;
            _postingService = postingService;
            _commentService = commentService;
            _dbContext = dbContext;

            InitializeComponent();
            LoadTrendsAsync();
        }

        private async void LoadTrendsAsync()
        {
            try
            {
                btnFindTrends.Enabled = false;
                btnFindTrends.Text = "Đang tìm trends...";

                _currentTrends = await _trendService.GetTrendsAsync();
                UpdateTrendsListView();

                btnFindTrends.Text = "Tìm Trends";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi load trends: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnFindTrends.Enabled = true;
            }
        }

        private void UpdateTrendsListView()
        {
            lvTrends.Items.Clear();
            foreach (var trend in _currentTrends)
            {
                var item = new ListViewItem(new[]
                {
                    trend.Topic,
                    trend.Platform,
                    trend.Score.ToString(),
                    trend.DetectedAt.ToString("dd/MM/yyyy HH:mm")
                })
                {
                    Tag = trend
                };
                lvTrends.Items.Add(item);
            }
        }

        private async void btnFindTrends_Click(object sender, EventArgs e)
        {
            LoadTrendsAsync();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            var settingsForm = Program.ServiceProvider.GetService(typeof(SettingsForm)) as SettingsForm;
            settingsForm?.ShowDialog();
        }

        private void lvTrends_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvTrends.SelectedItems.Count > 0)
            {
                _selectedTrend = lvTrends.SelectedItems[0].Tag as TrendingTopic;
                btnGenerateContent.Enabled = _selectedTrend != null;
            }
        }

        private async void btnGenerateContent_Click(object sender, EventArgs e)
        {
            if (_selectedTrend == null) return;

            try
            {
                btnGenerateContent.Enabled = false;
                btnGenerateContent.Text = "Đang tạo content...";

                _generatedContent = await _contentService.GenerateContentAsync(_selectedTrend);

                txtContent.Text = _generatedContent.Text;
                txtHashtags.Text = _generatedContent.Hashtags;

                btnPostNow.Enabled = true;
                btnSchedule.Enabled = true;

                MessageBox.Show("Đã tạo content thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo content: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnGenerateContent.Enabled = true;
                btnGenerateContent.Text = "Tạo Content";
            }
        }

        private async void btnPostNow_Click(object sender, EventArgs e)
        {
            if (_generatedContent == null) return;

            var platforms = new List<string>();
            if (chkFacebook.Checked) platforms.Add("Facebook");
            if (chkTikTok.Checked) platforms.Add("TikTok");
            if (chkYouTube.Checked) platforms.Add("YouTube");
            if (chkZalo.Checked) platforms.Add("Zalo");
            if (chkTwitter.Checked) platforms.Add("Twitter");

            if (platforms.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất 1 platform!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnPostNow.Enabled = false;
                btnPostNow.Text = "Đang đăng...";

                // Update content from textboxes
                _generatedContent.Text = txtContent.Text;
                _generatedContent.Hashtags = txtHashtags.Text;

                await _postingService.PostNowAsync(_generatedContent, platforms);

                MessageBox.Show($"Đã đăng thành công lên {platforms.Count} platforms!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Reset
                txtContent.Clear();
                txtHashtags.Clear();
                _generatedContent = null;
                btnPostNow.Enabled = false;
                btnSchedule.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đăng bài: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnPostNow.Enabled = true;
            }
            finally
            {
                btnPostNow.Text = "Đăng Ngay";
            }
        }

        private void btnSchedule_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng lên lịch đang được phát triển!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
