using System;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoDB.Driver;
using CourseSharesApp.Data;
using CourseSharesApp.Models;

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
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            var title = txtSearchTitle.Text;
            _currentMaterial = _context.Materials.Find(m => m.Title == title).FirstOrDefault();

            if (_currentMaterial != null)
            {
                lblInfo.Text = $"Found: {_currentMaterial.Title} (Status: {_currentMaterial.Status})";
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
            if (_currentMaterial == null) return;

            var update = Builders<Material>.Update.Set(m => m.Status, "Approved");
            _context.Materials.UpdateOne(m => m.Id == _currentMaterial.Id, update);

            MessageBox.Show("Material Approved!");
            this.Close();
        }
    }
}
