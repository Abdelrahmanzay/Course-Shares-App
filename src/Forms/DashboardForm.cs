using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoDB.Driver;
using CourseSharesApp.Data;

namespace CourseSharesApp.Forms
{
    public class DashboardForm : Form
    {
        private readonly DatabaseContext _ctx;
        private readonly DataGridView _grid;
        private readonly Button _btnTrending;
        private readonly Button _btnContributors;
        private readonly Button _btnPending;
        private readonly Button _btnSectionActivity;
        private readonly Button _btnInsert;
        private readonly Button _btnUpdate;
        private readonly Button _btnDelete;

        public DashboardForm(DatabaseContext ctx)
        {
            _ctx = ctx;
            Text = "Course Shares - Dashboard";
            Width = 1000;
            Height = 700;

            _grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

            var panel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 60 };

            _btnTrending = new Button { Text = "Trending Materials" };
            _btnContributors = new Button { Text = "Top Contributors" };
            _btnPending = new Button { Text = "Pending Queue" };
            _btnSectionActivity = new Button { Text = "Section Activity" };
            _btnInsert = new Button { Text = "Insert Material" };
            _btnUpdate = new Button { Text = "Approve Material" };
            _btnDelete = new Button { Text = "Delete Material" };

            _btnTrending.Click += (s, e) => RunTrendingMaterials();
            _btnContributors.Click += (s, e) => RunTopContributors();
            _btnPending.Click += (s, e) => RunPendingQueue();
            _btnSectionActivity.Click += (s, e) => RunSectionActivity();
            _btnInsert.Click += (s, e) => new InsertForm(_ctx).ShowDialog(this);
            _btnUpdate.Click += (s, e) => new UpdateForm(_ctx).ShowDialog(this);
            _btnDelete.Click += (s, e) => new DeleteForm(_ctx).ShowDialog(this);

            panel.Controls.AddRange(new Control[] { _btnTrending, _btnContributors, _btnPending, _btnSectionActivity, _btnInsert, _btnUpdate, _btnDelete });

            Controls.Add(_grid);
            Controls.Add(panel);
        }

        private void Bind(IEnumerable<BsonDocument> docs)
        {
            var table = new DataTable();
            var list = docs.ToList();
            if (!list.Any()) { _grid.DataSource = table; return; }

            var columns = list.First().Names.ToList();
            foreach (var name in columns) table.Columns.Add(name);
            foreach (var d in list)
            {
                var row = table.NewRow();
                foreach (var name in columns)
                {
                    row[name] = d.GetValue(name, BsonNull.Value)?.ToString() ?? string.Empty;
                }
                table.Rows.Add(row);
            }
            _grid.DataSource = table;
        }

        // Aggregation: Trending Materials
        private void RunTrendingMaterials()
        {
            try
            {
                var pipeline = new List<BsonDocument>
                {
                    new BsonDocument("$sort", new BsonDocument("views_count", -1)),
                    new BsonDocument("$limit", 5),
                    new BsonDocument("$lookup", new BsonDocument
                    {
                        { "from", "courses" },
                        { "localField", "course_id" },
                        { "foreignField", "_id" },
                        { "as", "course" }
                    }),
                    new BsonDocument("$addFields", new BsonDocument("course_title", new BsonDocument("$arrayElemAt", new BsonArray { "$course.title", 0 }))),
                    new BsonDocument("$project", new BsonDocument
                    {
                        { "_id", 1 },
                        { "title", 1 },
                        { "views_count", 1 },
                        { "course_title", 1 }
                    })
                };

                var result = _ctx.Materials.Aggregate<BsonDocument>(pipeline).ToEnumerable();
                Bind(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // Aggregation: Top Contributors
        private void RunTopContributors()
        {
            try
            {
                var pipeline = new List<BsonDocument>
                {
                    new BsonDocument("$group", new BsonDocument
                    {
                        { "_id", "$user_id" },
                        { "uploads", new BsonDocument("$sum", 1) }
                    }),
                    new BsonDocument("$lookup", new BsonDocument
                    {
                        { "from", "users" },
                        { "localField", "_id" },
                        { "foreignField", "_id" },
                        { "as", "user" }
                    }),
                    new BsonDocument("$addFields", new BsonDocument("user_name", new BsonDocument("$arrayElemAt", new BsonArray { "$user.name", 0 }))),
                    new BsonDocument("$project", new BsonDocument
                    {
                        { "user_id", "$_id" },
                        { "uploads", 1 },
                        { "user_name", 1 },
                        { "_id", 0 }
                    }),
                    new BsonDocument("$sort", new BsonDocument("uploads", -1))
                };

                var result = _ctx.Materials.Aggregate<BsonDocument>(pipeline).ToEnumerable();
                Bind(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // Aggregation: Pending Approval Queue
        private void RunPendingQueue()
        {
            try
            {
                var pipeline = new List<BsonDocument>
                {
                    new BsonDocument("$match", new BsonDocument("status", "Pending")),
                    new BsonDocument("$project", new BsonDocument
                    {
                        { "_id", 1 },
                        { "title", 1 },
                        { "upload_date", 1 },
                        { "user_id", 1 }
                    })
                };

                var result = _ctx.Materials.Aggregate<BsonDocument>(pipeline).ToEnumerable();
                Bind(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // Aggregation: Section Activity
        private void RunSectionActivity()
        {
            try
            {
                var pipeline = new List<BsonDocument>
                {
                    new BsonDocument("$group", new BsonDocument
                    {
                        { "_id", "$section_id" },
                        { "courses_count", new BsonDocument("$sum", 1) }
                    }),
                    new BsonDocument("$lookup", new BsonDocument
                    {
                        { "from", "sections" },
                        { "localField", "_id" },
                        { "foreignField", "_id" },
                        { "as", "section" }
                    }),
                    new BsonDocument("$addFields", new BsonDocument("section_name", new BsonDocument("$arrayElemAt", new BsonArray { "$section.section_name", 0 }))),
                    new BsonDocument("$project", new BsonDocument
                    {
                        { "section_id", "$_id" },
                        { "courses_count", 1 },
                        { "section_name", 1 },
                        { "_id", 0 }
                    }),
                    new BsonDocument("$sort", new BsonDocument("courses_count", -1))
                };

                var result = _ctx.Courses.Aggregate<BsonDocument>(pipeline).ToEnumerable();
                Bind(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}
