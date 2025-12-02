using System;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoDB.Driver;
using CourseSharesApp.Data;
using CourseSharesApp.Models;
using CourseSharesApp.Auth;

namespace CourseSharesApp.Forms.Materials
{
    public partial class DeleteMaterialForm : Form
    {
        private readonly DatabaseContext _context;

        public DeleteMaterialForm(DatabaseContext context)
        {
            InitializeComponent();
            _context = context;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var title = txtTitle.Text;
            
            // Find the material first
            var material = _context.Materials.Find(m => m.Title == title).FirstOrDefault();
            
            if (material == null)
            {
                MessageBox.Show("No material found with that title.");
                return;
            }
            
            // If user is a student, check if they own this material
            if (UserSession.CurrentUserRole.ToLower() == "student")
            {
                if (material.UserId != UserSession.CurrentUserId)
                {
                    MessageBox.Show("You can only delete your own materials.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            
            // Proceed with deletion
            var result = _context.Materials.DeleteOne(m => m.Title == title);

            if (result.DeletedCount > 0)
                MessageBox.Show("Deleted Successfully.");
            else
                MessageBox.Show("Failed to delete material.");

            this.Close();
        }
    }
}
