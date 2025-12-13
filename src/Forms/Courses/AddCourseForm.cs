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
            string title = txtTitle.Text.Trim();
            string code = txtCode.Text.Trim();
            string description = txtDescription.Text.Trim();

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(code))
            {
                MessageBox.Show("Please enter course title and code.");
                return;
            }

            if (cmbSection.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a section.");
                return;
            }

            ObjectId sectionId = ObjectId.Empty;
            // If placeholder is selected or value is empty, ask user to pick a real section
            if (cmbSection.SelectedItem is SectionItem si)
            {
                if (si.Id == ObjectId.Empty)
                {
                    MessageBox.Show("Please select a valid section (not the placeholder).");
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
                MessageBox.Show("Please select a valid section.");
                return;
            }

            Course newCourse = new Course
            {
                Title = title,
                Code = code,
                Description = description,
                SectionId = sectionId
            };

            try
            {
                await _context.AddCourseAsync(newCourse);
                MessageBox.Show("Course added successfully.");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to add course:\n" + ex.Message);
            }
        }
    }
}
