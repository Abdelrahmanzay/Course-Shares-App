using System;
using System.Windows.Forms;
using CourseSharesApp.Auth;
using CourseSharesApp.Data;
using CourseSharesApp.Models;
using MongoDB.Driver;

namespace CourseSharesApp.Forms.Auth
{
    public partial class LoginForm : Form
    {
        private readonly DatabaseContext _context;

        public LoginForm() : this(null) { }

        public LoginForm(DatabaseContext? context)
        {
            InitializeComponent();
            _context = context ?? new DatabaseContext();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                var email = txtEmail.Text.Trim();
                var password = txtPassword.Text; // plain-text comparison (phase 1)

                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Please enter email and password.", "Missing Info", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var user = _context.Users
                    .Find(Builders<User>.Filter.Eq(u => u.Email, email))
                    .FirstOrDefault();

                if (user == null || user.Password != password)
                {
                    MessageBox.Show("Invalid email or password.", "Authentication Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                UserSession.Login(user.Id, user.Role, user.Name);

                // Go to Dashboard
                var dashboard = new CourseSharesApp.Forms.DashboardForm(_context);
                dashboard.FormClosed += (s, _) => this.Close();
                this.Hide();
                dashboard.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnOpenSignUp_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (var signUp = new SignUpForm(_context))
            {
                var result = signUp.ShowDialog(this);
                if (result == DialogResult.OK && UserSession.IsLoggedIn)
                {
                    var dashboard = new CourseSharesApp.Forms.DashboardForm(_context);
                    dashboard.FormClosed += (s, _) => this.Close();
                    dashboard.Show();
                    return;
                }
            }
            // Still not logged in; show login again
            this.Show();
        }

        // Simple placeholder behavior
        private void Txt_Enter(object? sender, EventArgs e)
        {
            if (sender is TextBox tb && tb.Tag is string ph && tb.Text == ph)
            {
                tb.Text = string.Empty;
                tb.ForeColor = System.Drawing.Color.White;
                if (tb == txtPassword) txtPassword.UseSystemPasswordChar = true;
            }
        }

        private void Txt_Leave(object? sender, EventArgs e)
        {
            if (sender is TextBox tb && tb.Tag is string ph && string.IsNullOrWhiteSpace(tb.Text))
            {
                if (tb == txtPassword) txtPassword.UseSystemPasswordChar = false;
                tb.Text = ph;
                tb.ForeColor = System.Drawing.Color.Silver;
            }
        }
    }
}
