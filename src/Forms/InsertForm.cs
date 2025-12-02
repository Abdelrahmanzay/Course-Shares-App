using System;
using System.Linq;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoDB.Driver;
using CourseSharesApp.Data;
using CourseSharesApp.Models;

namespace CourseSharesApp.Forms
{
    public class InsertForm : Form
    {
        private readonly DatabaseContext _ctx;
        private TextBox _txtTitle;
        private ComboBox _cmbType;
        private ComboBox _cmbCourse;
        private TextBox _txtUserId;
        private Button _btnSave;

        public InsertForm(DatabaseContext ctx)
        {
            _ctx = ctx;
            Text = "Insert Material";
            Width = 400;
            Height = 300;

            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 5, AutoSize = true };
            layout.Controls.Add(new Label { Text = "Title" }, 0, 0);
            _txtTitle = new TextBox(); layout.Controls.Add(_txtTitle, 1, 0);

            layout.Controls.Add(new Label { Text = "Type" }, 0, 1);
            _cmbType = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbType.Items.AddRange(new object[] { "file", "link" });
            _cmbType.SelectedIndex = 0;
            layout.Controls.Add(_cmbType, 1, 1);

            layout.Controls.Add(new Label { Text = "Course" }, 0, 2);
            _cmbCourse = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            layout.Controls.Add(_cmbCourse, 1, 2);

            layout.Controls.Add(new Label { Text = "User Id (ObjectId)" }, 0, 3);
            _txtUserId = new TextBox(); layout.Controls.Add(_txtUserId, 1, 3);

            _btnSave = new Button { Text = "Save" };
            _btnSave.Click += (s, e) => Save();
            layout.Controls.Add(_btnSave, 1, 4);

            Controls.Add(layout);
            LoadCourses();
        }

        private void LoadCourses()
        {
            try
            {
                var courses = _ctx.Courses.Find(FilterDefinition<Course>.Empty).ToList();
                _cmbCourse.Items.Clear();
                foreach (var c in courses)
                {
                    _cmbCourse.Items.Add(new ComboItem { Text = c.Title, Value = c.Id });
                }
                if (_cmbCourse.Items.Count > 0) _cmbCourse.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading courses: {ex.Message}");
            }
        }

        private void Save()
        {
            try
            {
                if (!ObjectId.TryParse(_txtUserId.Text.Trim(), out var userId))
                {
                    MessageBox.Show("Invalid UserId ObjectId");
                    return;
                }

                var selectedCourse = _cmbCourse.SelectedItem as ComboItem;
                if (selectedCourse == null)
                {
                    MessageBox.Show("Select a course");
                    return;
                }

                var material = new Material
                {
                    Id = ObjectId.GenerateNewId(),
                    Title = _txtTitle.Text.Trim(),
                    Type = _cmbType.SelectedItem.ToString(),
                    UploadDate = DateTime.UtcNow,
                    Status = "Pending",
                    ViewsCount = 0,
                    UserId = userId,
                    CourseId = (ObjectId)selectedCourse.Value,
                    Comments = Enumerable.Empty<Comment>().ToList()
                };

                _ctx.Materials.InsertOne(material);
                MessageBox.Show("Material inserted.");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving: {ex.Message}");
            }
        }

        private class ComboItem
        {
            public string Text { get; set; }
            public object Value { get; set; }
            public override string ToString() => Text;
        }
    }
}
