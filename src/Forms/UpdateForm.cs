using System;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoDB.Driver;
using CourseSharesApp.Data;
using CourseSharesApp.Models;

namespace CourseSharesApp.Forms
{
    public class UpdateForm : Form
    {
        private readonly DatabaseContext _ctx;
        private TextBox _txtSearch;
        private RadioButton _rbById;
        private RadioButton _rbByTitle;
        private Button _btnApprove;

        public UpdateForm(DatabaseContext ctx)
        {
            _ctx = ctx;
            Text = "Approve Material";
            Width = 400;
            Height = 200;

            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 3 };
            _rbById = new RadioButton { Text = "Search by ID", Checked = true };
            _rbByTitle = new RadioButton { Text = "Search by Title" };
            var rbPanel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 30 };
            rbPanel.Controls.Add(_rbById);
            rbPanel.Controls.Add(_rbByTitle);

            layout.Controls.Add(new Label { Text = "Query" }, 0, 0);
            _txtSearch = new TextBox(); layout.Controls.Add(_txtSearch, 1, 0);

            layout.Controls.Add(rbPanel, 1, 1);

            _btnApprove = new Button { Text = "Approve" };
            _btnApprove.Click += (s, e) => Approve();
            layout.Controls.Add(_btnApprove, 1, 2);

            Controls.Add(layout);
        }

        private void Approve()
        {
            try
            {
                FilterDefinition<Material> filter;
                if (_rbById.Checked)
                {
                    if (!ObjectId.TryParse(_txtSearch.Text.Trim(), out var id))
                    {
                        MessageBox.Show("Invalid ObjectId");
                        return;
                    }
                    filter = Builders<Material>.Filter.Eq(x => x.Id, id);
                }
                else
                {
                    filter = Builders<Material>.Filter.Eq(x => x.Title, _txtSearch.Text.Trim());
                }

                var update = Builders<Material>.Update.Set(x => x.Status, "Approved");
                var result = _ctx.Materials.UpdateOne(filter, update);
                if (result.ModifiedCount > 0)
                    MessageBox.Show("Material approved.");
                else
                    MessageBox.Show("No matching material found.");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error approving: {ex.Message}");
            }
        }
    }
}
