using System;
using System.Linq;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoDB.Driver;
using CourseSharesApp.Data;
using CourseSharesApp.Models;

namespace CourseSharesApp.Forms.Courses
{
    public class AddCourseForm : Form
    {
        private readonly DatabaseContext _context;

        private TextBox txtTitle;
        private TextBox txtCode;
        private TextBox txtDescription;
        private ComboBox cmbSection;
        private Button btnSubmit;

        public AddCourseForm(DatabaseContext context)
        {
            _context = context;
            InitializeComponent();
        }

        // Helper class for ComboBox binding
        private class SectionItem
        {
            public ObjectId Id { get; set; }
            public string Name { get; set; }

            public override string ToString() => Name;
        }

        private void InitializeComponent()
        {
            this.Text = "Add Course";
            this.Width = 400;
            this.Height = 330;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;

            Label lblTitle = new Label { Text = "Title:", Left = 12, Top = 20, Width = 80 };
            txtTitle = new TextBox { Left = 100, Top = 18, Width = 260 };

            Label lblCode = new Label { Text = "Code:", Left = 12, Top = 60, Width = 80 };
            txtCode = new TextBox { Left = 100, Top = 58, Width = 260 };

            Label lblSection = new Label { Text = "Section:", Left = 12, Top = 100, Width = 80 };
            cmbSection = new ComboBox
            {
                Left = 100,
                Top = 98,
                Width = 260,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            Label lblDesc = new Label { Text = "Description:", Left = 12, Top = 140, Width = 80 };
            txtDescription = new TextBox
            {
                Left = 100,
                Top = 138,
                Width = 260,
                Height = 70,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            btnSubmit = new Button
            {
                Text = "Add Course",
                Left = 100,
                Top = 225,
                Width = 120
            };
            btnSubmit.Click += BtnSubmit_Click;

            Controls.Add(lblTitle);
            Controls.Add(txtTitle);
            Controls.Add(lblCode);
            Controls.Add(txtCode);
            Controls.Add(lblSection);
            Controls.Add(cmbSection);
            Controls.Add(lblDesc);
            Controls.Add(txtDescription);
            Controls.Add(btnSubmit);

            this.Load += AddCourseForm_Load;
        }

        private async void AddCourseForm_Load(object sender, EventArgs e)
        {
            try
            {
                if (_context == null)
                {
                    MessageBox.Show("Error: Database context is null.");
                    cmbSection.Enabled = false;
                    return;
                }

                var sections = await _context.Sections.Find(_ => true).ToListAsync();

                if (sections != null && sections.Count > 0)
                {
                    var sectionItems = sections.Select(s => new SectionItem
                    {
                        Id = s.Id,
                        Name = s.SectionName ?? "(no name)"
                    }).ToList();

                    var placeholder = new SectionItem { Id = ObjectId.Empty, Name = "-- Select Section --" };
                    sectionItems.Insert(0, placeholder);

                    cmbSection.DisplayMember = "Name";
                    cmbSection.ValueMember = "Id";
                    cmbSection.DataSource = sectionItems;
                    cmbSection.SelectedIndex = 0;
                }
                else
                {
                    cmbSection.Items.Clear();
                    cmbSection.Items.Add("-- no sections available --");
                    cmbSection.SelectedIndex = 0;
                    cmbSection.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading sections:\n{ex.GetType().Name}: {ex.Message}");
                cmbSection.Enabled = false;
            }
        }

        private async void BtnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                string title = txtTitle.Text.Trim();
                string code = txtCode.Text.Trim();
                string description = txtDescription.Text.Trim();

                // Validate title
                if (string.IsNullOrWhiteSpace(title))
                {
                    MessageBox.Show("Please enter a course title.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTitle.Focus();
                    return;
                }

                // Validate title length
                if (title.Length < 3)
                {
                    MessageBox.Show("Course title must be at least 3 characters long.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTitle.Focus();
                    return;
                }

                if (title.Length > 150)
                {
                    MessageBox.Show("Course title is too long. Please limit it to 150 characters.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTitle.Focus();
                    return;
                }

                // Validate code
                if (string.IsNullOrWhiteSpace(code))
                {
                    MessageBox.Show("Please enter a course code.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCode.Focus();
                    return;
                }

                // Validate code length (MongoDB requires minimum 4 characters)
                if (code.Length < 4)
                {
                    MessageBox.Show("Course code must be at least 4 characters long.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCode.Focus();
                    return;
                }

                // Validate code format (alphanumeric and common separators)
                if (!System.Text.RegularExpressions.Regex.IsMatch(code, @"^[A-Za-z0-9\-_]+$"))
                {
                    MessageBox.Show("Course code can only contain letters, numbers, hyphens, and underscores.\n\nExample: CS101, MATH-201, ENG_300", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCode.Focus();
                    return;
                }

                if (code.Length > 20)
                {
                    MessageBox.Show("Course code must not exceed 20 characters.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCode.Focus();
                    return;
                }

                // Validate description
                if (string.IsNullOrWhiteSpace(description))
                {
                    MessageBox.Show("Please enter a course description.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDescription.Focus();
                    return;
                }

                if (description.Length < 10)
                {
                    MessageBox.Show("Course description must be at least 10 characters long.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDescription.Focus();
                    return;
                }

                if (description.Length > 1000)
                {
                    MessageBox.Show("Course description is too long. Please limit it to 1000 characters.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDescription.Focus();
                    return;
                }

                // Validate section selection
                if (cmbSection.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select a section for this course.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbSection.Focus();
                    return;
                }

                ObjectId sectionId = ObjectId.Empty;
                // If placeholder is selected or value is empty, ask user to pick a real section
                if (cmbSection.SelectedItem is SectionItem si)
                {
                    if (si.Id == ObjectId.Empty)
                    {
                        MessageBox.Show("Please select a valid section from the list.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        cmbSection.Focus();
                        return;
                    }
                    sectionId = si.Id;
                }
                else if (cmbSection.SelectedValue is ObjectId oid && oid != ObjectId.Empty)
                {
                    sectionId = oid;
                }
                else
                {
                    MessageBox.Show("Please select a valid section from the list.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbSection.Focus();
                    return;
                }

                // Check for duplicate course code
                var existingCourse = await _context.Courses
                    .Find(c => c.Code.ToLower() == code.ToLower())
                    .FirstOrDefaultAsync();

                if (existingCourse != null)
                {
                    MessageBox.Show($"A course with the code '{code}' already exists.\n\nPlease use a different course code.", "Duplicate Course Code", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCode.Focus();
                    return;
                }

                // Check for duplicate course title in the same section
                var duplicateTitle = await _context.Courses
                    .Find(c => c.Title.ToLower() == title.ToLower() && c.SectionId == sectionId)
                    .FirstOrDefaultAsync();

                if (duplicateTitle != null)
                {
                    var result = MessageBox.Show(
                        $"A course with the title '{title}' already exists in this section.\n\nDo you want to proceed anyway?",
                        "Duplicate Course Title",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.No)
                    {
                        txtTitle.Focus();
                        return;
                    }
                }

                // Create course object
                Course newCourse = new Course
                {
                    Title = title,
                    Code = code,
                    Description = description,
                    SectionId = sectionId
                };

                // Save to database
                await _context.AddCourseAsync(newCourse);
                MessageBox.Show($"Course '{title}' (Code: {code}) added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (MongoWriteException mongoEx)
            {
                // Handle MongoDB validation errors specifically (error code 121)
                if (mongoEx.WriteError?.Code == 121)
                {
                    string errorMsg = "The course could not be saved due to validation errors:\n\n";
                    
                    if (mongoEx.Message.Contains("code"))
                    {
                        errorMsg += "• Course code must be at least 4 characters long\n";
                    }
                    if (mongoEx.Message.Contains("title"))
                    {
                        errorMsg += "• Course title must meet minimum length requirements\n";
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
                    MessageBox.Show($"Database error while adding course:\n{mongoEx.Message}\n\nPlease try again or contact support.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (MongoConnectionException connEx)
            {
                MessageBox.Show($"Unable to connect to the database:\n{connEx.Message}\n\nPlease check your internet connection and try again.", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred while adding the course:\n{ex.Message}\n\nPlease try again or contact support.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
