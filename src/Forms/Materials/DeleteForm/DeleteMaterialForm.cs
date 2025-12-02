using System;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoDB.Driver;
using CourseSharesApp.Data;
using CourseSharesApp.Models;

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
            var result = _context.Materials.DeleteOne(m => m.Title == title);

            if (result.DeletedCount > 0)
                MessageBox.Show("Deleted Successfully.");
            else
                MessageBox.Show("No material found with that title.");

            this.Close();
        }
    }
}
