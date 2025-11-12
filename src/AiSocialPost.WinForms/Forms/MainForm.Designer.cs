namespace AiSocialPost.WinForms.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private Button btnFindTrends;
        private Button btnSettings;
        private ListView lvTrends;
        private Button btnGenerateContent;
        private TextBox txtContent;
        private TextBox txtHashtags;
        private CheckBox chkFacebook;
        private CheckBox chkTikTok;
        private CheckBox chkYouTube;
        private CheckBox chkZalo;
        private CheckBox chkTwitter;
        private Button btnPostNow;
        private Button btnSchedule;
        private Label lblTrends;
        private Label lblContent;
        private Label lblHashtags;
        private Label lblPlatforms;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnFindTrends = new Button();
            this.btnSettings = new Button();
            this.lvTrends = new ListView();
            this.btnGenerateContent = new Button();
            this.txtContent = new TextBox();
            this.txtHashtags = new TextBox();
            this.chkFacebook = new CheckBox();
            this.chkTikTok = new CheckBox();
            this.chkYouTube = new CheckBox();
            this.chkZalo = new CheckBox();
            this.chkTwitter = new CheckBox();
            this.btnPostNow = new Button();
            this.btnSchedule = new Button();
            this.lblTrends = new Label();
            this.lblContent = new Label();
            this.lblHashtags = new Label();
            this.lblPlatforms = new Label();

            this.SuspendLayout();

            // Main Form
            this.ClientSize = new Size(1000, 700);
            this.Text = "AI Social Post - Tool Đăng Bài Tự Động";
            this.StartPosition = FormStartPosition.CenterScreen;

            // btnFindTrends
            this.btnFindTrends.Location = new Point(20, 20);
            this.btnFindTrends.Size = new Size(120, 35);
            this.btnFindTrends.Text = "Tìm Trends";
            this.btnFindTrends.Click += btnFindTrends_Click;

            // btnSettings
            this.btnSettings.Location = new Point(860, 20);
            this.btnSettings.Size = new Size(120, 35);
            this.btnSettings.Text = "Settings";
            this.btnSettings.Click += btnSettings_Click;

            // lblTrends
            this.lblTrends.Location = new Point(20, 70);
            this.lblTrends.Size = new Size(200, 20);
            this.lblTrends.Text = "Trends (Click để chọn):";
            this.lblTrends.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            // lvTrends
            this.lvTrends.Location = new Point(20, 95);
            this.lvTrends.Size = new Size(460, 250);
            this.lvTrends.View = View.Details;
            this.lvTrends.FullRowSelect = true;
            this.lvTrends.GridLines = true;
            this.lvTrends.Columns.Add("Topic", 200);
            this.lvTrends.Columns.Add("Platform", 80);
            this.lvTrends.Columns.Add("Score", 60);
            this.lvTrends.Columns.Add("Detected", 120);
            this.lvTrends.SelectedIndexChanged += lvTrends_SelectedIndexChanged;

            // btnGenerateContent
            this.btnGenerateContent.Location = new Point(20, 360);
            this.btnGenerateContent.Size = new Size(150, 35);
            this.btnGenerateContent.Text = "Tạo Content";
            this.btnGenerateContent.Enabled = false;
            this.btnGenerateContent.Click += btnGenerateContent_Click;

            // lblContent
            this.lblContent.Location = new Point(500, 70);
            this.lblContent.Size = new Size(200, 20);
            this.lblContent.Text = "Content (có thể edit):";
            this.lblContent.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            // txtContent
            this.txtContent.Location = new Point(500, 95);
            this.txtContent.Size = new Size(480, 200);
            this.txtContent.Multiline = true;
            this.txtContent.ScrollBars = ScrollBars.Vertical;

            // lblHashtags
            this.lblHashtags.Location = new Point(500, 310);
            this.lblHashtags.Size = new Size(200, 20);
            this.lblHashtags.Text = "Hashtags:";
            this.lblHashtags.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            // txtHashtags
            this.txtHashtags.Location = new Point(500, 335);
            this.txtHashtags.Size = new Size(480, 25);

            // lblPlatforms
            this.lblPlatforms.Location = new Point(20, 420);
            this.lblPlatforms.Size = new Size(200, 20);
            this.lblPlatforms.Text = "Chọn Platforms:";
            this.lblPlatforms.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            // chkFacebook
            this.chkFacebook.Location = new Point(20, 450);
            this.chkFacebook.Size = new Size(100, 25);
            this.chkFacebook.Text = "Facebook";
            this.chkFacebook.Checked = true;

            // chkTikTok
            this.chkTikTok.Location = new Point(130, 450);
            this.chkTikTok.Size = new Size(100, 25);
            this.chkTikTok.Text = "TikTok";

            // chkYouTube
            this.chkYouTube.Location = new Point(240, 450);
            this.chkYouTube.Size = new Size(100, 25);
            this.chkYouTube.Text = "YouTube";

            // chkZalo
            this.chkZalo.Location = new Point(350, 450);
            this.chkZalo.Size = new Size(100, 25);
            this.chkZalo.Text = "Zalo";

            // chkTwitter
            this.chkTwitter.Location = new Point(460, 450);
            this.chkTwitter.Size = new Size(100, 25);
            this.chkTwitter.Text = "Twitter/X";

            // btnPostNow
            this.btnPostNow.Location = new Point(20, 500);
            this.btnPostNow.Size = new Size(150, 40);
            this.btnPostNow.Text = "Đăng Ngay";
            this.btnPostNow.BackColor = Color.FromArgb(0, 122, 255);
            this.btnPostNow.ForeColor = Color.White;
            this.btnPostNow.Enabled = false;
            this.btnPostNow.Click += btnPostNow_Click;

            // btnSchedule
            this.btnSchedule.Location = new Point(190, 500);
            this.btnSchedule.Size = new Size(150, 40);
            this.btnSchedule.Text = "Lên Lịch";
            this.btnSchedule.Enabled = false;
            this.btnSchedule.Click += btnSchedule_Click;

            // Add controls to form
            this.Controls.Add(this.btnFindTrends);
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.lblTrends);
            this.Controls.Add(this.lvTrends);
            this.Controls.Add(this.btnGenerateContent);
            this.Controls.Add(this.lblContent);
            this.Controls.Add(this.txtContent);
            this.Controls.Add(this.lblHashtags);
            this.Controls.Add(this.txtHashtags);
            this.Controls.Add(this.lblPlatforms);
            this.Controls.Add(this.chkFacebook);
            this.Controls.Add(this.chkTikTok);
            this.Controls.Add(this.chkYouTube);
            this.Controls.Add(this.chkZalo);
            this.Controls.Add(this.chkTwitter);
            this.Controls.Add(this.btnPostNow);
            this.Controls.Add(this.btnSchedule);

            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
