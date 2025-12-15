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
            try
            {
                // 1. Validate Input (using txtTitle and txtDescription from the designer)
                string title = txtTitle.Text.Trim();
                string description = txtDescription.Text.Trim();

                // Validate section name
                if (string.IsNullOrWhiteSpace(title))
                {
                    MessageBox.Show("Please enter a section name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTitle.Focus();
                    return;
                }

                // Validate section name length
                if (title.Length < 3)
                {
                    MessageBox.Show("Section name must be at least 3 characters long.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTitle.Focus();
                    return;
                }

                if (title.Length > 100)
                {
                    MessageBox.Show("Section name is too long. Please limit it to 100 characters.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTitle.Focus();
                    return;
                }

                // Validate section name contains valid characters
                if (!System.Text.RegularExpressions.Regex.IsMatch(title, @"^[A-Za-z0-9\s\-_&]+$"))
                {
                    MessageBox.Show("Section name contains invalid characters.\n\nAllowed: letters, numbers, spaces, hyphens, underscores, and ampersands.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTitle.Focus();
                    return;
                }

                // Validate description
                if (string.IsNullOrWhiteSpace(description))
                {
                    MessageBox.Show("Please enter a section description.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDescription.Focus();
                    return;
                }

                // Validate description length
                if (description.Length < 10)
                {
                    MessageBox.Show("Section description must be at least 10 characters long.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDescription.Focus();
                    return;
                }

                if (description.Length > 500)
                {
                    MessageBox.Show("Section description is too long. Please limit it to 500 characters.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDescription.Focus();
                    return;
                }

                // 2. Check for duplicate section name (case-insensitive)
                var existingSection = await _context.Sections
                    .Find(s => s.SectionName.ToLower() == title.ToLower())
                    .FirstOrDefaultAsync();

                if (existingSection != null)
                {
                    MessageBox.Show($"A section named '{title}' already exists.\n\nPlease use a different name.", "Duplicate Section", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTitle.Focus();
                    return;
                }

                // 3. Create Model instance
                var newSection = new Section
                {
                    SectionName = title,
                    Description = description
                };

                // 4. Call the Data Layer method
                await _context.AddSectionAsync(newSection);
                MessageBox.Show($"Section '{title}' added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (MongoWriteException mongoEx)
            {
                // Handle MongoDB validation errors specifically (error code 121)
                if (mongoEx.WriteError?.Code == 121)
                {
                    string errorMsg = "The section could not be saved due to validation errors:\n\n";
                    
                    if (mongoEx.Message.Contains("sectionName") || mongoEx.Message.Contains("title"))
                    {
                        errorMsg += "• Section name must meet minimum length requirements\n";
                    }
                    if (mongoEx.Message.Contains("description"))
                    {
                        errorMsg += "• Section description must meet minimum length requirements\n";
                    }
                    if (mongoEx.Message.Contains("minLength"))
                    {
                        errorMsg += "• One or more fields don't meet minimum length requirements\n";
                    }
                    if (mongoEx.Message.Contains("required"))
                    {
                        errorMsg += "• All required fields must be filled\n";
                    }
                    
                    errorMsg += "\nPlease check your input and try again.";
                    MessageBox.Show(errorMsg, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show($"Database error while adding section:\n{mongoEx.Message}\n\nPlease try again or contact support.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (MongoConnectionException connEx)
            {
                MessageBox.Show($"Unable to connect to the database:\n{connEx.Message}\n\nPlease check your internet connection and try again.", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred while adding the section:\n{ex.Message}\n\nPlease try again or contact support.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}