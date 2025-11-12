namespace AiSocialPost.WinForms.Forms
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox txtGeminiApiKey;
        private TextBox txtFacebookAppId;
        private TextBox txtFacebookAppSecret;
        private TextBox txtTikTokClientKey;
        private TextBox txtYouTubeClientId;
        private TextBox txtZaloAppId;
        private TextBox txtTwitterApiKey;
        private TextBox txtBananaApiKey;
        private Button btnSave;
        private Button btnCancel;
        private Button btnTestConnection;
        private Label lblGemini;
        private Label lblFacebook;
        private Label lblTikTok;
        private Label lblYouTube;
        private Label lblZalo;
        private Label lblTwitter;
        private Label lblBanana;

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
            this.txtGeminiApiKey = new TextBox();
            this.txtFacebookAppId = new TextBox();
            this.txtFacebookAppSecret = new TextBox();
            this.txtTikTokClientKey = new TextBox();
            this.txtYouTubeClientId = new TextBox();
            this.txtZaloAppId = new TextBox();
            this.txtTwitterApiKey = new TextBox();
            this.txtBananaApiKey = new TextBox();
            this.btnSave = new Button();
            this.btnCancel = new Button();
            this.btnTestConnection = new Button();
            this.lblGemini = new Label();
            this.lblFacebook = new Label();
            this.lblTikTok = new Label();
            this.lblYouTube = new Label();
            this.lblZalo = new Label();
            this.lblTwitter = new Label();
            this.lblBanana = new Label();

            this.SuspendLayout();

            // Settings Form
            this.ClientSize = new Size(600, 550);
            this.Text = "Settings - API Configuration";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            int yPos = 20;
            int labelWidth = 150;
            int textBoxX = 170;
            int textBoxWidth = 400;

            // Gemini API Key
            this.lblGemini.Location = new Point(20, yPos);
            this.lblGemini.Size = new Size(labelWidth, 20);
            this.lblGemini.Text = "Gemini API Key:";
            this.txtGeminiApiKey.Location = new Point(textBoxX, yPos - 3);
            this.txtGeminiApiKey.Size = new Size(textBoxWidth, 25);
            yPos += 40;

            // Facebook
            this.lblFacebook.Location = new Point(20, yPos);
            this.lblFacebook.Size = new Size(labelWidth, 20);
            this.lblFacebook.Text = "Facebook App ID:";
            this.txtFacebookAppId.Location = new Point(textBoxX, yPos - 3);
            this.txtFacebookAppId.Size = new Size(textBoxWidth, 25);
            yPos += 35;

            var lblFacebookSecret = new Label();
            lblFacebookSecret.Location = new Point(20, yPos);
            lblFacebookSecret.Size = new Size(labelWidth, 20);
            lblFacebookSecret.Text = "Facebook App Secret:";
            this.txtFacebookAppSecret.Location = new Point(textBoxX, yPos - 3);
            this.txtFacebookAppSecret.Size = new Size(textBoxWidth, 25);
            this.txtFacebookAppSecret.UseSystemPasswordChar = true;
            yPos += 40;

            // TikTok
            this.lblTikTok.Location = new Point(20, yPos);
            this.lblTikTok.Size = new Size(labelWidth, 20);
            this.lblTikTok.Text = "TikTok Client Key:";
            this.txtTikTokClientKey.Location = new Point(textBoxX, yPos - 3);
            this.txtTikTokClientKey.Size = new Size(textBoxWidth, 25);
            yPos += 40;

            // YouTube
            this.lblYouTube.Location = new Point(20, yPos);
            this.lblYouTube.Size = new Size(labelWidth, 20);
            this.lblYouTube.Text = "YouTube Client ID:";
            this.txtYouTubeClientId.Location = new Point(textBoxX, yPos - 3);
            this.txtYouTubeClientId.Size = new Size(textBoxWidth, 25);
            yPos += 40;

            // Zalo
            this.lblZalo.Location = new Point(20, yPos);
            this.lblZalo.Size = new Size(labelWidth, 20);
            this.lblZalo.Text = "Zalo App ID:";
            this.txtZaloAppId.Location = new Point(textBoxX, yPos - 3);
            this.txtZaloAppId.Size = new Size(textBoxWidth, 25);
            yPos += 40;

            // Twitter
            this.lblTwitter.Location = new Point(20, yPos);
            this.lblTwitter.Size = new Size(labelWidth, 20);
            this.lblTwitter.Text = "Twitter API Key:";
            this.txtTwitterApiKey.Location = new Point(textBoxX, yPos - 3);
            this.txtTwitterApiKey.Size = new Size(textBoxWidth, 25);
            yPos += 40;

            // Banana API
            this.lblBanana.Location = new Point(20, yPos);
            this.lblBanana.Size = new Size(labelWidth, 20);
            this.lblBanana.Text = "Banana API Key:";
            this.txtBananaApiKey.Location = new Point(textBoxX, yPos - 3);
            this.txtBananaApiKey.Size = new Size(textBoxWidth, 25);
            yPos += 50;

            // Buttons
            this.btnSave.Location = new Point(170, yPos);
            this.btnSave.Size = new Size(100, 35);
            this.btnSave.Text = "Lưu";
            this.btnSave.BackColor = Color.FromArgb(0, 122, 255);
            this.btnSave.ForeColor = Color.White;
            this.btnSave.Click += btnSave_Click;

            this.btnTestConnection.Location = new Point(280, yPos);
            this.btnTestConnection.Size = new Size(130, 35);
            this.btnTestConnection.Text = "Test Connection";
            this.btnTestConnection.Click += btnTestConnection_Click;

            this.btnCancel.Location = new Point(420, yPos);
            this.btnCancel.Size = new Size(100, 35);
            this.btnCancel.Text = "Hủy";
            this.btnCancel.Click += btnCancel_Click;

            // Add controls
            this.Controls.Add(this.lblGemini);
            this.Controls.Add(this.txtGeminiApiKey);
            this.Controls.Add(this.lblFacebook);
            this.Controls.Add(this.txtFacebookAppId);
            this.Controls.Add(lblFacebookSecret);
            this.Controls.Add(this.txtFacebookAppSecret);
            this.Controls.Add(this.lblTikTok);
            this.Controls.Add(this.txtTikTokClientKey);
            this.Controls.Add(this.lblYouTube);
            this.Controls.Add(this.txtYouTubeClientId);
            this.Controls.Add(this.lblZalo);
            this.Controls.Add(this.txtZaloAppId);
            this.Controls.Add(this.lblTwitter);
            this.Controls.Add(this.txtTwitterApiKey);
            this.Controls.Add(this.lblBanana);
            this.Controls.Add(this.txtBananaApiKey);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnTestConnection);
            this.Controls.Add(this.btnCancel);

            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
