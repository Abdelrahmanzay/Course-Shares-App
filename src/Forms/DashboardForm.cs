using System;
using System.Linq;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoDB.Driver;
using CourseSharesApp.Data;
using CourseSharesApp.Forms.Materials;

namespace CourseSharesApp.Forms
{
    public partial class DashboardForm : Form
    {
        private readonly DatabaseContext _context;

        public DashboardForm(DatabaseContext context)
        {
            InitializeComponent();
            _context = context;
        }

        private async void btnTrending_Click(object sender, EventArgs e)
        {
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$sort", new BsonDocument("viewsCount", -1)),
                new BsonDocument("$limit", 5),
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "courses" },
                    { "localField", "courseId" },
                    { "foreignField", "_id" },
                    { "as", "courseInfo" }
                }),
                new BsonDocument("$unwind", "$courseInfo"),
                new BsonDocument("$project", new BsonDocument
                {
                    { "Title", "$title" },
                    { "Views", "$viewsCount" },
                    { "Course", "$courseInfo.title" },
                    { "Status", "$status" },
                    { "_id", 0 }
                })
            };
            await RunAggregation("materials", pipeline);
        }

        private async void btnPending_Click(object sender, EventArgs e)
        {
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$match", new BsonDocument("status", "Pending")),
                new BsonDocument("$lookup", new BsonDocument { { "from", "users" }, { "localField", "uploadedBy" }, { "foreignField", "_id" }, { "as", "uploader" } }),
                new BsonDocument("$unwind", "$uploader"),
                new BsonDocument("$project", new BsonDocument { { "Material", "$title" }, { "UploadedAt", "$uploadDate" }, { "By", "$uploader.name" }, { "_id", 0 } })
            };
            await RunAggregation("materials", pipeline);
        }

        private async void btnContributors_Click(object sender, EventArgs e)
        {
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$group", new BsonDocument { { "_id", "$uploadedBy" }, { "UploadCount", new BsonDocument("$sum", 1) } }),
                new BsonDocument("$sort", new BsonDocument("UploadCount", -1)),
                new BsonDocument("$lookup", new BsonDocument { { "from", "users" }, { "localField", "_id" }, { "foreignField", "_id" }, { "as", "user" } }),
                new BsonDocument("$unwind", "$user"),
                new BsonDocument("$project", new BsonDocument { { "Name", "$user.name" }, { "Uploads", "$UploadCount" }, { "_id", 0 } })
            };
            await RunAggregation("materials", pipeline);
        }

        private async void btnSections_Click(object sender, EventArgs e)
        {
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$group", new BsonDocument { { "_id", "$sectionId" }, { "CourseCount", new BsonDocument("$sum", 1) } }),
                new BsonDocument("$lookup", new BsonDocument { { "from", "sections" }, { "localField", "_id" }, { "foreignField", "_id" }, { "as", "section" } }),
                new BsonDocument("$unwind", "$section"),
                new BsonDocument("$project", new BsonDocument { { "Section", "$section.sectionName" }, { "TotalCourses", "$CourseCount" }, { "_id", 0 } })
            };
            await RunAggregation("courses", pipeline);
        }

        private async System.Threading.Tasks.Task RunAggregation(string collectionName, BsonDocument[] pipeline)
        {
            try
            {
                var collection = _context.Materials.Database.GetCollection<BsonDocument>(collectionName);
                var result = await collection.Aggregate<BsonDocument>(pipeline).ToListAsync();

                // Convert BsonDocuments to a DataTable-like structure or list of dynamic objects for DataGridView
                var displayList = result.Select(doc => doc.ToDictionary()).ToList();

                if (displayList.Count > 0)
                {
                    // Create a DataTable manually to handle BsonValues gracefully
                    var dt = new System.Data.DataTable();
                    foreach (var key in displayList[0].Keys) dt.Columns.Add(key);

                    foreach (var row in displayList)
                    {
                        var dr = dt.NewRow();
                        foreach (var key in row.Keys) dr[key] = row[key].ToString();
                        dt.Rows.Add(dr);
                    }
                    dgvResults.DataSource = dt;
                }
                else
                {
                    dgvResults.DataSource = null;
                    MessageBox.Show("No results found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void btnOpenInsert_Click(object sender, EventArgs e) => new InsertMaterialForm(_context).ShowDialog();
        private void btnOpenUpdate_Click(object sender, EventArgs e) => new UpdateMaterialForm(_context).ShowDialog();
        private void btnOpenDelete_Click(object sender, EventArgs e) => new DeleteMaterialForm(_context).ShowDialog();
    }
}