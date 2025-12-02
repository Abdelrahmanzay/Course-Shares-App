using System;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoDB.Driver;
using CourseSharesApp.Data;
using CourseSharesApp.Models;

namespace CourseSharesApp.Forms
{
    public class DeleteForm : Form
    {
        private readonly DatabaseContext _ctx;
        private TextBox _txtId;
        private Button _btnDelete;

        public DeleteForm(DatabaseContext ctx)
        {
            _ctx = ctx;
            Text = "Delete Material";
            Width = 350;
            Height = 150;

            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2 };
            layout.Controls.Add(new Label { Text = "Material ID" }, 0, 0);
            _txtId = new TextBox(); layout.Controls.Add(_txtId, 1, 0);

            _btnDelete = new Button { Text = "Delete" };
            _btnDelete.Click += (s, e) => Delete();
            layout.Controls.Add(_btnDelete, 1, 1);

            Controls.Add(layout);
        }

        private void Delete()
        {
            try
            {
                if (!ObjectId.TryParse(_txtId.Text.Trim(), out var id))
                {
                    MessageBox.Show("Invalid ObjectId");
                    return;
                }

                var result = _ctx.Materials.DeleteOne(Builders<Material>.Filter.Eq(x => x.Id, id));
                if (result.DeletedCount > 0)
                    MessageBox.Show("Material deleted.");
                else
                    MessageBox.Show("No matching material found.");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting: {ex.Message}");
            }
        }
    }
}
