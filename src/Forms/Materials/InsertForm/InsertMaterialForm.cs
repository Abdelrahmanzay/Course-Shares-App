using System;
using System.IO; // Required for FileStream
using System.Linq;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoDB.Driver;

using CourseSharesApp.Data;
using CourseSharesApp.Models;

namespace CourseSharesApp.Forms.Materials
{
    public partial class InsertMaterialForm : Form
    {
        private readonly DatabaseContext _context;
        private string _selectedFilePath; // Stores the path of the file to be uploaded

        public InsertMaterialForm(DatabaseContext context)
        {
            InitializeComponent();
            _context = context;
            LoadCourses();
        }

        private void LoadCourses()
        {
            var courses = _context.Courses.Find(_ => true).ToList();
            cmbCourse.DataSource = courses;
            cmbCourse.DisplayMember = "Title";
            cmbCourse.ValueMember = "Id";
        }

        // New Event Handler for the Browse Button
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Select Material File";
                // Allow PDF, Word, PowerPoint, and common documents
                ofd.Filter = "Documents|*.pdf;*.doc;*.docx;*.ppt;*.pptx;*.txt|All Files|*.*";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _selectedFilePath = ofd.FileName;
                    // Display only the file name in the text box for user feedback
                    txtLink.Text = Path.GetFileName(_selectedFilePath);

                    // Auto-select the "File" radio button
                    rbFile.Checked = true;
                }
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text) || cmbCourse.SelectedItem == null)
            {
                MessageBox.Show("Required fields missing: Title and Course are mandatory.");
                return;
            }

            string finalLinkOrId = txtLink.Text.Trim();

            try
            {
                // Logic for File selection without GridFS (store path as link)
                if (rbFile.Checked)
                {
                    if (string.IsNullOrEmpty(_selectedFilePath) || !File.Exists(_selectedFilePath))
                    {
                        MessageBox.Show("Please click 'Browse' and select a valid file to upload.", "File Missing");
                        return;
                    }
                    // Do NOT upload to GridFS. Save the full path as the link.
                    finalLinkOrId = _selectedFilePath;
                }
                // Logic for Web Link
                else if (rbLink.Checked)
                {
                    if (!Uri.IsWellFormedUriString(finalLinkOrId, UriKind.Absolute))
                    {
                        MessageBox.Show("Please enter a valid absolute URI (e.g., https://example.com/file.pdf).", "Invalid Link", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Create the material object
                var material = new Material
                {
                    Title = txtTitle.Text,
                    Type = rbFile.Checked ? "file" : "link",
                    FileLink = finalLinkOrId, // Stores either the local file path or the URL
                    UploadDate = DateTime.Now,
                    Status = "Pending",
                    ViewsCount = 0,
                    CourseId = (ObjectId)cmbCourse.SelectedValue,
                    UserId = CourseSharesApp.Auth.UserSession.CurrentUserId,
                    Comments = new List<Comment>()
                };

                // Save to database
                _context.Materials.InsertOne(material);

                MessageBox.Show("Material uploaded/saved successfully.");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving material: {ex.Message}", "Database Error");
            }
        }
    }
}