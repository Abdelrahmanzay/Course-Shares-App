using System;
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text) || cmbCourse.SelectedItem == null)
            {
                MessageBox.Show("Required fields missing.");
                return;
            }

            var material = new Material
            {
                Title = txtTitle.Text,
                Type = rbFile.Checked ? "file" : "link",
                FileLink = txtLink.Text,
                UploadDate = DateTime.Now,
                Status = "Pending",
                ViewsCount = 0,
                CourseId = (ObjectId)cmbCourse.SelectedValue,
                UserId = _context.Users.Find(_ => true).First().Id
            };

            _context.Materials.InsertOne(material);
            MessageBox.Show("Material inserted successfully.");
            this.Close();
        }
    }
}
