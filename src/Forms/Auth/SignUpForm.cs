using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CourseSharesApp.Data;
using CourseSharesApp.Models;
using CourseSharesApp.Auth;
// using CourseSharesApp.Forms.DashBoard;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CourseSharesApp.Forms.Auth
{
    public partial class SignUpForm : Form
    {
        private readonly DatabaseContext _context;

        public SignUpForm(DatabaseContext context)
        {
            InitializeComponent();
            _context = context ?? new DatabaseContext();
        }

        private void btnSignUp_Click(object sender, EventArgs e)
        {
            try
            {
                var name = txtName.Text.Trim();
                var email = txtEmail.Text.Trim();
                var password = txtPassword.Text;
                var city = txtCity.Text.Trim();
                var country = txtCountry.Text.Trim();
                var postalCode = txtPostalCode.Text.Trim();

                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)
                    || string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(country) || string.IsNullOrWhiteSpace(postalCode))
                {
                    MessageBox.Show("Please fill in all fields.", "Missing Info", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Name validation: not a number
                if (double.TryParse(name, out _))
                {
                    MessageBox.Show("Name cannot be a number.", "Invalid Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Password validation: more than 5 characters
                if (password.Length <= 5)
                {
                    MessageBox.Show("Password must be more than 5 characters.", "Invalid Password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Email validation: simple regex
                if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    MessageBox.Show("Please enter a valid email address.", "Invalid Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Address validation
                if (!System.Text.RegularExpressions.Regex.IsMatch(city, @"^[A-Za-z\s'-]{2,}$"))
                {
                    MessageBox.Show("City must contain letters only.", "Invalid City", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!System.Text.RegularExpressions.Regex.IsMatch(country, @"^[A-Za-z\s'-]{2,}$"))
                {
                    MessageBox.Show("Country must contain letters only.", "Invalid Country", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!System.Text.RegularExpressions.Regex.IsMatch(postalCode, @"^[A-Za-z0-9\s-]{3,10}$"))
                {
                    MessageBox.Show("Postal Code must be 3-10 letters/numbers.", "Invalid Postal Code", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var existing = _context.Users.Find(Builders<User>.Filter.Eq(u => u.Email, email)).FirstOrDefault();
                if (existing != null)
                {
                    MessageBox.Show("Email already registered.", "Duplicate Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var user = new User
                {
                    Id = ObjectId.GenerateNewId(),
                    Name = name,
                    Email = email,
                    Password = password,
                    Role = "student",
                    DateJoined = DateTime.Now,
                    Address = new Address { City = city, Country = country, PostalCode = postalCode },
                    SearchHistory = new List<SearchHistoryItem>()
                };

                _context.Users.InsertOne(user);
                MessageBox.Show("Account created successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Set session and return success to caller (LoginForm)
                UserSession.Login(user.Id, user.Role, user.Name);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Sign up error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
