using System;
using System.Windows.Forms;
using MongoDB.Driver;
using CourseSharesApp.Data;
using CourseSharesApp.Models;

// IMPORTANT: Ensure the namespace matches your project structure
namespace CourseSharesApp.Forms.Sections
{
    public partial class AddSectionForm : Form
    {
        private readonly DatabaseContext _context;

        public AddSectionForm(DatabaseContext dbContext)
        {
            // InitializeComponent is called here by the framework
            InitializeComponent();
            _context = dbContext;
        }

        private async void btnAddSectionSubmit_Click(object sender, EventArgs e)
        {
            // 1. Validate Input (using txtTitle and txtDescription from the designer)
            string title = txtTitle.Text.Trim();
            string description = txtDescription.Text.Trim();

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(description))
            {
                MessageBox.Show("Please enter both the section title and description.", "Input Required");
                return;
            }

            // 2. Check for duplicate section name
            try
            {
                var existingSection = _context.Sections.Find(s => s.SectionName == title).FirstOrDefault();
                if (existingSection != null)
                {
                    MessageBox.Show($"A section named '{title}' already exists. Please use a different name.", "Duplicate Section", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking for duplicate section: {ex.Message}", "Database Error");
                return;
            }

            // 3. Create Model instance
            var newSection = new Section
            {
                SectionName = title,
                Description = description
            };

            // 4. Call the Data Layer method
            try
            {
                await _context.AddSectionAsync(newSection); // Corresponds to Action 7: Add Section [cite: 59]
                MessageBox.Show($"Section '{title}' added successfully!", "Success");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                // In a real app, you might log the full exception details
                MessageBox.Show($"Failed to add section: {ex.Message}", "Database Error");
            }
        }
    }
}