using System.Drawing;
using System.Windows.Forms;

namespace CourseSharesApp.Forms.Auth
{
    partial class SignUpForm
    {
        private System.ComponentModel.IContainer components = null;
        private Panel pnl;
        private Label lblTitle;
        private TextBox txtName;
        private TextBox txtEmail;
        private TextBox txtPassword;
        private TextBox txtCity;
        private TextBox txtCountry;
        private TextBox txtPostalCode;
        private Button btnSignUp;
        private Button btnBack;

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            this.Text = "Course Shares - Sign Up";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.ClientSize = new Size(560, 560);

            var bg = ColorTranslator.FromHtml("#293541");
            var btnBg = ColorTranslator.FromHtml("#333F4B");
            var btnHover = ColorTranslator.FromHtml("#3E4B58");

            this.BackColor = bg;

            pnl = new Panel
            {
                BackColor = Color.FromArgb(30, 30, 38),
                Size = new Size(400, 420),
                Left = (this.ClientSize.Width - 400) / 2,
                Top = (this.ClientSize.Height - 420) / 2,
                AutoScroll = true
            };
            this.Controls.Add(pnl);

            lblTitle = new Label
            {
                Text = "Create Your Account",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 60
            };
            pnl.Controls.Add(lblTitle);

            txtName = new TextBox
            {
                Left = 60,
                Top = 80,
                Width = 280,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(45, 52, 62),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtName.PlaceholderText = "Full Name";
            pnl.Controls.Add(txtName);

            txtEmail = new TextBox
            {
                Left = 60,
                Top = 120,
                Width = 280,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(45, 52, 62),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtEmail.PlaceholderText = "Email";
            pnl.Controls.Add(txtEmail);

            txtPassword = new TextBox
            {
                Left = 60,
                Top = 160,
                Width = 280,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(45, 52, 62),
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = true
            };
            txtPassword.PlaceholderText = "Password";
            pnl.Controls.Add(txtPassword);

            txtCity = new TextBox
            {
                Left = 60,
                Top = 200,
                Width = 280,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(45, 52, 62),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtCity.PlaceholderText = "City";
            pnl.Controls.Add(txtCity);

            txtCountry = new TextBox
            {
                Left = 60,
                Top = 240,
                Width = 280,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(45, 52, 62),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtCountry.PlaceholderText = "Country";
            pnl.Controls.Add(txtCountry);

            txtPostalCode = new TextBox
            {
                Left = 60,
                Top = 280,
                Width = 280,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(45, 52, 62),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtPostalCode.PlaceholderText = "Postal Code";
            pnl.Controls.Add(txtPostalCode);

            btnSignUp = new Button
            {
                Text = "Sign Up",
                Left = 60,
                Top = 330,
                Width = 280,
                Height = 36,
                BackColor = btnBg,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSignUp.FlatAppearance.BorderSize = 0;
            btnSignUp.FlatAppearance.MouseOverBackColor = btnHover;
            btnSignUp.Click += btnSignUp_Click;
            pnl.Controls.Add(btnSignUp);

            btnBack = new Button
            {
                Text = "Back",
                Left = 60,
                Top = 374,
                Width = 280,
                Height = 32,
                BackColor = Color.FromArgb(38, 46, 56),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.FlatAppearance.MouseOverBackColor = btnHover;
            btnBack.Click += btnBack_Click;
            pnl.Controls.Add(btnBack);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
