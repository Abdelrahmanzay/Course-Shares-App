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
            try
            {
                // Validate title
                if (string.IsNullOrWhiteSpace(txtTitle.Text))
                {
                    MessageBox.Show("Please enter a material title.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTitle.Focus();
                    return;
                }

                // Validate title length (MongoDB requires minimum 3 characters)
                if (txtTitle.Text.Trim().Length < 3)
                {
                    MessageBox.Show("Material title must be at least 3 characters long.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTitle.Focus();
                    return;
                }

                if (txtTitle.Text.Trim().Length > 200)
                {
                    MessageBox.Show("Material title is too long. Please limit it to 200 characters.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTitle.Focus();
                    return;
                }

                // Validate course selection
                if (cmbCourse.SelectedItem == null || cmbCourse.SelectedValue == null)
                {
                    MessageBox.Show("Please select a course for this material.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbCourse.Focus();
                    return;
                }

                // Validate that either file or link radio button is selected
                if (!rbFile.Checked && !rbLink.Checked)
                {
                    MessageBox.Show("Please select whether you're uploading a file or providing a link.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string finalLinkOrId = txtLink.Text.Trim();

                // Logic for File selection
                if (rbFile.Checked)
                {
                    // Validate file path exists
                    if (string.IsNullOrWhiteSpace(_selectedFilePath))
                    {
                        MessageBox.Show("Please click 'Browse' and select a file to upload.", "File Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        btnBrowse.Focus();
                        return;
                    }

                    // Validate file exists
                    if (!File.Exists(_selectedFilePath))
                    {
                        MessageBox.Show($"The selected file no longer exists at the location:\n{_selectedFilePath}\n\nPlease select a different file.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        _selectedFilePath = string.Empty;
                        txtLink.Clear();
                        return;
                    }

                    // Validate file size (max 100MB)
                    var fileInfo = new FileInfo(_selectedFilePath);
                    if (fileInfo.Length > 100 * 1024 * 1024)
                    {
                        MessageBox.Show("The selected file is too large. Maximum file size is 100 MB.", "File Too Large", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Validate file extension
                    string[] allowedExtensions = { ".pdf", ".doc", ".docx", ".ppt", ".pptx", ".txt" };
                    string fileExtension = Path.GetExtension(_selectedFilePath).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        MessageBox.Show($"File type '{fileExtension}' is not supported.\n\nAllowed file types: PDF, Word (DOC/DOCX), PowerPoint (PPT/PPTX), and TXT.", "Invalid File Type", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    finalLinkOrId = _selectedFilePath;
                }
                // Logic for Web Link
                else if (rbLink.Checked)
                {
                    // Validate link field is not empty
                    if (string.IsNullOrWhiteSpace(finalLinkOrId))
                    {
                        MessageBox.Show("Please enter a web link (URL) for the material.", "Link Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtLink.Focus();
                        return;
                    }

                    // Validate URL format
                    if (!Uri.IsWellFormedUriString(finalLinkOrId, UriKind.Absolute))
                    {
                        MessageBox.Show("Please enter a valid web link.\n\nExample: https://example.com/file.pdf", "Invalid Link Format", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtLink.Focus();
                        return;
                    }

                    // Validate URL scheme (only http or https)
                    Uri uri = new Uri(finalLinkOrId);
                    if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
                    {
                        MessageBox.Show("Please enter a valid HTTP or HTTPS link.", "Invalid Link Protocol", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtLink.Focus();
                        return;
                    }

                    // Validate URL length
                    if (finalLinkOrId.Length > 2000)
                    {
                        MessageBox.Show("The URL is too long. Please use a shorter link.", "URL Too Long", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Validate current user session
                if (CourseSharesApp.Auth.UserSession.CurrentUserId == ObjectId.Empty)
                {
                    MessageBox.Show("User session is invalid. Please log in again.", "Session Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Check for duplicate material title in the same course
                var existingMaterial = await _context.Materials
                    .Find(m => m.Title == txtTitle.Text.Trim() && m.CourseId == (ObjectId)cmbCourse.SelectedValue)
                    .FirstOrDefaultAsync();

                if (existingMaterial != null)
                {
                    var result = MessageBox.Show(
                        $"A material with the title '{txtTitle.Text.Trim()}' already exists in this course.\n\nDo you want to proceed anyway?",
                        "Duplicate Material Title",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.No)
                    {
                        return;
                    }
                }

                // Create the material object
                var material = new Material
                {
                    Title = txtTitle.Text.Trim(),
                    Type = rbFile.Checked ? "file" : "link",
                    FileLink = finalLinkOrId,
                    UploadDate = DateTime.Now,
                    Status = "Pending",
                    ViewsCount = 0,
                    CourseId = (ObjectId)cmbCourse.SelectedValue,
                    UserId = CourseSharesApp.Auth.UserSession.CurrentUserId,
                    Comments = new List<Comment>()
                };

                // Save to database
                await _context.Materials.InsertOneAsync(material);

                MessageBox.Show("Material uploaded successfully!\n\nStatus: Pending approval", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (MongoWriteException mongoEx)
            {
                // Handle MongoDB validation errors specifically (error code 121)
                if (mongoEx.WriteError?.Code == 121)
                {
                    string errorMsg = "The material could not be saved due to validation errors:\n\n";
                    
                    if (mongoEx.Message.Contains("title"))
                    {
                        errorMsg += "• Title must be at least 3 characters long\n";
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
                    MessageBox.Show($"Database error while saving material:\n{mongoEx.Message}\n\nPlease try again or contact support.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (MongoConnectionException connEx)
            {
                MessageBox.Show($"Unable to connect to the database:\n{connEx.Message}\n\nPlease check your internet connection and try again.", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (IOException ioEx)
            {
                MessageBox.Show($"File system error:\n{ioEx.Message}\n\nPlease check file permissions and try again.", "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (UnauthorizedAccessException uaEx)
            {
                MessageBox.Show($"Access denied to the file:\n{uaEx.Message}\n\nPlease check file permissions.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred:\n{ex.Message}\n\nPlease try again or contact support.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}