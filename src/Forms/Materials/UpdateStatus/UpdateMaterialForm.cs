using System;
using System.Windows.Forms;
using MongoDB.Driver;
using CourseSharesApp.Data;
using CourseSharesApp.Models;
using CourseSharesApp.Auth;

namespace CourseSharesApp.Forms.Materials
{
    public partial class UpdateMaterialForm : Form
    {
        private readonly DatabaseContext _context;
        private Material _currentMaterial;

        public UpdateMaterialForm(DatabaseContext context)
        {
            InitializeComponent();
            _context = context;

            btnApprove.Enabled = false; // Disabled until search
        }


        private void btnSearch_Click(object sender, EventArgs e)
        {
            // Block students
            if (UserSession.CurrentUserRole.ToLower() == "student")
            {
                MessageBox.Show("Students do not have permission to approve materials.",
                                "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var title = txtSearchTitle.Text.Trim();

            if (string.IsNullOrEmpty(title))
            {
                MessageBox.Show("Please enter a material title to search.");
                return;
            }

            _currentMaterial = _context.Materials.Find(m => m.Title == title).FirstOrDefault();

            if (_currentMaterial != null)
            {
                lblInfo.Text = $"Found: {_currentMaterial.Title} (Status: {_currentMaterial.Status})";

                // We ONLY check approval inside the Approve button, so here we ALWAYS enable it
                btnApprove.Enabled = true;
            }
            else
            {
                lblInfo.Text = "Material not found.";
                btnApprove.Enabled = false;
            }
        }


        private void btnApprove_Click(object sender, EventArgs e)
        {
            if (_currentMaterial == null)
                return;

            // 🔥 CHECK HERE (not in search)
            if (_currentMaterial.Status == "Approved")
            {
                MessageBox.Show("This material is already approved.",
                                "Already Approved", MessageBoxButtons.OK, MessageBoxIcon.Information);

                btnApprove.Enabled = false;  
                return;
            }

            // Approve the material
            var update = Builders<Material>.Update.Set(m => m.Status, "Approved");
            _context.Materials.UpdateOne(m => m.Id == _currentMaterial.Id, update);

            MessageBox.Show("Material Approved Successfully!");

            btnApprove.Enabled = false; // Prevent approving again
            this.Close();
        }
    }
}
