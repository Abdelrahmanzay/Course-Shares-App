using System.Drawing;
using System.Windows.Forms;

namespace CourseSharesApp.Forms.Auth
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;
        private Panel pnl;
        private Label lblTitle;
        private TextBox txtEmailPlaceholder;
        private TextBox txtPasswordPlaceholder;
        private Button btnLogin;
        private Button btnOpenSignUp;

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            this.Text = "Course Shares - Login";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.ClientSize = new Size(480, 360);

            var bg = ColorTranslator.FromHtml("#293541");
            var btnBg = ColorTranslator.FromHtml("#333F4B");
            var btnHover = ColorTranslator.FromHtml("#3E4B58");

            this.BackColor = bg;

            pnl = new Panel
            {
                BackColor = Color.FromArgb(30, 30, 38),
                Size = new Size(360, 260),
                Left = (this.ClientSize.Width - 360) / 2,
                Top = (this.ClientSize.Height - 260) / 2
            };
            this.Controls.Add(pnl);

            lblTitle = new Label
            {
                Text = "Course Shares Login",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 60
            };
            pnl.Controls.Add(lblTitle);

            txtEmail = new TextBox
            {
                Left = 40,
                Top = 80,
                Width = 280,
                ForeColor = Color.Silver,
                BackColor = Color.FromArgb(45, 52, 62),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtEmail.Tag = "Email";
            txtEmail.Text = (string)txtEmail.Tag;
            txtEmail.Enter += Txt_Enter;
            txtEmail.Leave += Txt_Leave;
            pnl.Controls.Add(txtEmail);

            txtPassword = new TextBox
            {
                Left = 40,
                Top = 120,
                Width = 280,
                ForeColor = Color.Silver,
                BackColor = Color.FromArgb(45, 52, 62),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtPassword.Tag = "Password";
            txtPassword.Text = (string)txtPassword.Tag;
            txtPassword.UseSystemPasswordChar = false;
            txtPassword.Enter += Txt_Enter;
            txtPassword.Leave += Txt_Leave;
            pnl.Controls.Add(txtPassword);

            btnLogin = new Button
            {
                Text = "Login",
                Left = 40,
                Top = 170,
                Width = 280,
                Height = 36,
                BackColor = btnBg,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.FlatAppearance.MouseOverBackColor = btnHover;
            btnLogin.Click += btnLogin_Click;
            pnl.Controls.Add(btnLogin);

            btnOpenSignUp = new Button
            {
                Text = "Create Account",
                Left = 40,
                Top = 214,
                Width = 280,
                Height = 32,
                BackColor = Color.FromArgb(38, 46, 56),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnOpenSignUp.FlatAppearance.BorderSize = 0;
            btnOpenSignUp.FlatAppearance.MouseOverBackColor = btnHover;
            btnOpenSignUp.Click += btnOpenSignUp_Click;
            pnl.Controls.Add(btnOpenSignUp);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private TextBox txtEmail;
        private TextBox txtPassword;
    }
}
