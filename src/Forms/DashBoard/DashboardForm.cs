using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO; // Added for File handling
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS; // Added for GridFS
using CourseSharesApp.Data;
using CourseSharesApp.Forms.Materials;
using CourseSharesApp.Forms.Auth;
using CourseSharesApp.Auth;
using CourseSharesApp.Models;
using CourseSharesApp.Forms.Sections;
using CourseSharesApp.Services;

namespace CourseSharesApp.Forms
{
    public partial class DashboardForm : Form
    {
        private readonly DatabaseContext _context;
        private readonly ReportService _reportService;

        public DashboardForm(DatabaseContext context)
        {
            InitializeComponent();
            _context = context;
            _reportService = new ReportService(_context);

            if (UserSession.CurrentUserRole.ToLower() == "student")
            {
                btnOpenUpdate.Visible = false;
                btnOpenDelete.Visible = false;
                btnPending.Visible = false;

                try { btnOpenAddSection.Visible = false; } catch { }
                try { btnOpenAddCourse.Visible = false; } catch { }
            }

            if (UserSession.CurrentUserRole.ToLower() != "admin")
            {
                try { btnOpenAddSection.Visible = false; } catch { }
                try { btnOpenAddCourse.Visible = false; } catch { }
            }

            try
            {
                cmbUserList.Visible = false;
                cmbUserList.DisplayMember = "Name";
                cmbUserList.ValueMember = "Id";
                cmbUserList.SelectedIndexChanged -= cmbUserList_SelectedIndexChanged;
                cmbUserList.SelectedIndexChanged += cmbUserList_SelectedIndexChanged;

                cmbUserList.Location = new Point(txtSearch.Location.X, txtSearch.Location.Y + txtSearch.Height + 6);
                cmbUserList.Width = Math.Max(220, txtSearch.Width + 40);
            }
            catch { }

            LoadApprovedMaterials();
            dgvResults.CellContentClick += DgvResults_CellContentClick;
        }

        // ------------------- LOAD MATERIALS -------------------
        private async void LoadApprovedMaterials()
        {
            try
            {
                // Ensure we only load approved materials
                var projected = await _context.Materials
                    .Find(m => m.Status == "Approved")
                    .Project(Builders<CourseSharesApp.Models.Material>.Projection
                        .Include(x => x.Title)
                        .Include(x => x.ViewsCount)
                        .Include(x => x.UploadDate)
                        .Include(x => x.Status)
                        .Include(x => x.FileLink))
                    .ToListAsync();
                BindAggregationResult(projected);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // ------------------- AGGREGATION HELPER -------------------
        private void BindAggregationResult(System.Collections.Generic.List<BsonDocument> result)
        {
            try { cmbUserList.Visible = false; } catch { }

            var displayList = result.Select(doc => doc.ToDictionary()).ToList();

            if (displayList.Count > 0)
            {
                var dt = new System.Data.DataTable();
                foreach (var key in displayList[0].Keys) dt.Columns.Add(key);

                foreach (var row in displayList)
                {
                    var dr = dt.NewRow();
                    foreach (var key in row.Keys) dr[key] = row[key]?.ToString() ?? "";
                    dt.Rows.Add(dr);
                }

                dgvResults.DataSource = dt;

                if (dt.Columns.Contains("FileLink"))
                {
                    if (!(dgvResults.Columns["FileLink"] is DataGridViewLinkColumn))
                    {
                        dgvResults.Columns.Remove("FileLink");
                        var linkColumn = new DataGridViewLinkColumn
                        {
                            Name = "FileLink",
                            HeaderText = "File/Action",
                            DataPropertyName = "FileLink",
                            TrackVisitedState = true,
                            LinkColor = System.Drawing.Color.Blue,
                            ActiveLinkColor = System.Drawing.Color.Red,
                            VisitedLinkColor = System.Drawing.Color.Purple
                        };
                        dgvResults.Columns.Add(linkColumn);
                    }
                    if (dgvResults.Columns.Contains("_id"))
                    {
                        dgvResults.Columns["_id"].Visible = false;
                    }
                }
            }
            else
            {
                dgvResults.DataSource = null;
                MessageBox.Show("No results found.");
            }
        }

        // ------------------- HANDLE HYPERLINK CLICK (DOWNLOAD OR OPEN) -------------------
        private async void DgvResults_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvResults.Columns[e.ColumnIndex] is DataGridViewLinkColumn)
            {
                var linkVal = dgvResults.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();
                if (string.IsNullOrEmpty(linkVal)) return;

                // --- 1. Increment Views Logic (Preserved) ---
                try
                {
                    string idStr = null;
                    if (dgvResults.Columns.Contains("_id"))
                        idStr = dgvResults.Rows[e.RowIndex].Cells["_id"].Value?.ToString();

                    var materialsColl = _context.Materials.Database.GetCollection<BsonDocument>("materials");

                    if (!string.IsNullOrEmpty(idStr))
                    {
                        UpdateResult upd = null;
                        if (ObjectId.TryParse(idStr, out var matOid))
                        {
                            var filter = Builders<BsonDocument>.Filter.Eq("_id", matOid);
                            var update = Builders<BsonDocument>.Update.Inc("viewsCount", 1);
                            upd = await materialsColl.UpdateOneAsync(filter, update);
                        }
                        else
                        {
                            var filter = Builders<BsonDocument>.Filter.Eq("_id", idStr);
                            var update = Builders<BsonDocument>.Update.Inc("viewsCount", 1);
                            upd = await materialsColl.UpdateOneAsync(filter, update);

                            if (upd?.ModifiedCount == 0)
                            {
                                var linkFilter = Builders<BsonDocument>.Filter.Eq("fileLink", linkVal);
                                upd = await materialsColl.UpdateOneAsync(linkFilter, Builders<BsonDocument>.Update.Inc("viewsCount", 1));
                            }
                        }

                        if (upd != null && upd.ModifiedCount > 0)
                        {
                            try
                            {
                                if (dgvResults.Columns.Contains("ViewsCount"))
                                {
                                    var cell = dgvResults.Rows[e.RowIndex].Cells["ViewsCount"];
                                    if (int.TryParse(cell.Value?.ToString(), out var cur)) cell.Value = (cur + 1).ToString();
                                }
                                else if (dgvResults.Columns.Contains("Views"))
                                {
                                    var cell = dgvResults.Rows[e.RowIndex].Cells["Views"];
                                    if (int.TryParse(cell.Value?.ToString(), out var cur)) cell.Value = (cur + 1).ToString();
                                }
                            }
                            catch { }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"viewsCount update failed: {ex.Message}");
                }

                // --- 2. Handle File Download vs Web Link ---
                // If the linkVal is a MongoDB ObjectId, it is a file in GridFS.
                if (ObjectId.TryParse(linkVal, out ObjectId gridFSId))
                {
                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        string title = "download";
                        if (dgvResults.Columns.Contains("Title"))
                            title = dgvResults.Rows[e.RowIndex].Cells["Title"].Value?.ToString();

                        // Sanitize filename
                        foreach (char c in Path.GetInvalidFileNameChars())
                            title = title.Replace(c, '_');

                        sfd.FileName = title;
                        // Default filter; users can choose extension if they know it
                        sfd.Filter = "All Files|*.*|PDF Files|*.pdf|Images|*.jpg;*.png";

                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            try
                            {
                                using (var stream = File.Create(sfd.FileName))
                                {
                                    // Download from GridFS
                                    await _context.Bucket.DownloadToStreamAsync(gridFSId, stream);
                                }

                                if (MessageBox.Show("Download complete. Open file?", "Success", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    Process.Start(new ProcessStartInfo { FileName = sfd.FileName, UseShellExecute = true });
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Download failed: {ex.Message}");
                            }
                        }
                    }
                }
                else
                {
                    // It is a standard web URL
                    try
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = linkVal,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Cannot open link: {ex.Message}");
                    }
                }
            }
        }

        private void btnHome_Click(object sender, EventArgs e) => LoadApprovedMaterials();

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();

            if (string.IsNullOrEmpty(searchTerm))
            {
                LoadApprovedMaterials();
                return;
            }

            try
            {
                var userId = UserSession.CurrentUserId;
                var filter = Builders<User>.Filter.Eq("_id", userId);

                var searchHistoryDoc = new BsonDocument
                {
                    { "keyword", searchTerm },
                    { "timestamp", DateTime.Now }
                };

                var update = Builders<User>.Update.Push("searchHistory", searchHistoryDoc);
                _context.Users.UpdateOne(filter, update);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to save search history: {ex.Message}");
            }

            var regex = new BsonRegularExpression(searchTerm, "i");
            var docs = await _reportService.SearchApprovedMaterialsAsync(regex);
            BindAggregationResult(docs);
        }

        private async void btnViewUserUploads_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> userIdStrings = new List<string>();

                try
                {
                    var projected = await _context.Materials
                        .Find(_ => true)
                        .Project(m => m.UserId)
                        .ToListAsync();

                    if (projected != null)
                    {
                        foreach (var x in projected)
                        {
                            if (x == null) continue;
                            userIdStrings.Add(x.ToString());
                        }
                    }
                }
                catch { }

                if (userIdStrings.Count == 0)
                {
                    var materialsColl = _context.Materials.Database.GetCollection<BsonDocument>("materials");
                    var distinctCursor = await materialsColl.DistinctAsync<BsonValue>("uploadedBy", FilterDefinition<BsonDocument>.Empty);
                    var distinctValues = (await distinctCursor.ToListAsync()).Where(v => v != BsonNull.Value).ToList();

                    foreach (var v in distinctValues)
                    {
                        if (v.IsObjectId) userIdStrings.Add(v.AsObjectId.ToString());
                        else if (v.IsString) userIdStrings.Add(v.AsString);
                    }
                }

                userIdStrings = userIdStrings.Distinct().ToList();

                if (userIdStrings.Count == 0)
                {
                    MessageBox.Show("No users with uploads found.");
                    return;
                }

                var usersColl = _context.Materials.Database.GetCollection<BsonDocument>("users");
                var objectIds = userIdStrings.Where(id => ObjectId.TryParse(id, out _)).Select(id => ObjectId.Parse(id)).ToList();
                var stringIds = userIdStrings.Except(objectIds.Select(o => o.ToString())).ToList();

                var filters = new List<FilterDefinition<BsonDocument>>();
                if (objectIds.Count > 0) filters.Add(Builders<BsonDocument>.Filter.In("_id", objectIds));
                if (stringIds.Count > 0) filters.Add(Builders<BsonDocument>.Filter.In("_id", stringIds));

                var finalFilter = filters.Count > 0 ? Builders<BsonDocument>.Filter.Or(filters) : Builders<BsonDocument>.Filter.Empty;
                var users = await usersColl.Find(finalFilter).Project(Builders<BsonDocument>.Projection.Include("_id").Include("name")).ToListAsync();

                var list = userIdStrings.Select(id =>
                {
                    var matched = users.FirstOrDefault(u =>
                        (u.Contains("_id") && u["_id"].BsonType == BsonType.ObjectId && ObjectId.TryParse(id, out var oid) && u["_id"].AsObjectId == oid)
                        || (u.Contains("_id") && u["_id"].BsonType == BsonType.String && u["_id"].AsString == id)
                    );

                    return new UserItem
                    {
                        Id = id,
                        Name = matched != null && matched.Contains("name") ? matched["name"].AsString : id
                    };
                }).OrderBy(u => u.Name).ToList();

                cmbUserList.DataSource = list;
                cmbUserList.DisplayMember = "Name";
                cmbUserList.ValueMember = "Id";

                try
                {
                    cmbUserList.Location = new Point(txtSearch.Location.X, txtSearch.Location.Y + txtSearch.Height + 6);
                    cmbUserList.Width = Math.Max(220, txtSearch.Width + 40);
                }
                catch { }

                cmbUserList.BringToFront();
                cmbUserList.Focus();
                cmbUserList.Visible = true;
                cmbUserList.DroppedDown = true;

                try
                {
                    int desiredTop = cmbUserList.Location.Y + cmbUserList.Height + 6;
                    int oldTop = dgvResults.Top;
                    if (oldTop < desiredTop)
                    {
                        int delta = desiredTop - oldTop;
                        dgvResults.Top = desiredTop;
                        dgvResults.Height = Math.Max(120, dgvResults.Height - delta);
                    }
                }
                catch { }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading users: " + ex.Message);
            }
        }

        private async void cmbUserList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbUserList.SelectedItem == null) return;
                if (!(cmbUserList.SelectedItem is UserItem selected)) return;

                var materialsColl = _context.Materials.Database.GetCollection<BsonDocument>("materials");
                var filters = new List<FilterDefinition<BsonDocument>>();

                if (ObjectId.TryParse(selected.Id, out var parsedOid))
                {
                    filters.Add(Builders<BsonDocument>.Filter.Eq("uploadedBy", parsedOid));
                }

                filters.Add(Builders<BsonDocument>.Filter.Eq("userId", selected.Id));
                filters.Add(Builders<BsonDocument>.Filter.Eq("UserId", selected.Id));

                var combined = Builders<BsonDocument>.Filter.Or(filters);
                var approvedOnly = Builders<BsonDocument>.Filter.Eq("status", "Approved");
                var docs = await materialsColl.Find(Builders<BsonDocument>.Filter.And(combined, approvedOnly)).ToListAsync();

                if (docs == null || docs.Count == 0)
                {
                    dgvResults.DataSource = null;
                    MessageBox.Show("This user has no uploaded materials.");
                    return;
                }

                var dt = new System.Data.DataTable();
                dt.Columns.Add("Title");
                dt.Columns.Add("UploadedBy");
                dt.Columns.Add("FileLink");
                dt.Columns.Add("_id");

                var usersColl = _context.Materials.Database.GetCollection<BsonDocument>("users");

                foreach (var d in docs)
                {
                    var title = d.Contains("title") ? d["title"].ToString() : "";
                    string uploaderName = string.Empty;

                    if (d.Contains("uploadedBy") && d["uploadedBy"].BsonType == BsonType.ObjectId)
                    {
                        var oid = d["uploadedBy"].AsObjectId;
                        var u = await usersColl.Find(Builders<BsonDocument>.Filter.Eq("_id", oid)).Project(Builders<BsonDocument>.Projection.Include("name")).FirstOrDefaultAsync();
                        if (u != null && u.Contains("name")) uploaderName = u["name"].AsString;
                    }
                    else if (d.Contains("uploader") && d["uploader"].IsBsonDocument && d["uploader"].AsBsonDocument.Contains("name"))
                    {
                        uploaderName = d["uploader"].AsBsonDocument["name"].AsString;
                    }

                    var fileLink = d.Contains("fileLink") ? d["fileLink"].ToString() : "";
                    var idVal = d.Contains("_id") ? (d["_id"].IsObjectId ? d["_id"].AsObjectId.ToString() : d["_id"].ToString()) : string.Empty;
                    var dr = dt.NewRow();
                    dr["Title"] = title;
                    dr["UploadedBy"] = uploaderName;
                    dr["FileLink"] = fileLink;
                    dr["_id"] = idVal;
                    dt.Rows.Add(dr);
                }
                dgvResults.DataSource = dt;

                if (dt.Columns.Contains("FileLink"))
                {
                    if (dgvResults.Columns.Contains("FileLink")) dgvResults.Columns.Remove("FileLink");

                    var linkColumn = new DataGridViewLinkColumn
                    {
                        Name = "FileLink",
                        HeaderText = "File/Action",
                        DataPropertyName = "FileLink",
                        TrackVisitedState = true,
                        LinkColor = System.Drawing.Color.Blue,
                        ActiveLinkColor = System.Drawing.Color.Red,
                        VisitedLinkColor = System.Drawing.Color.Purple
                    };
                    dgvResults.Columns.Add(linkColumn);
                    if (dgvResults.Columns.Contains("_id")) dgvResults.Columns["_id"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading materials: " + ex.Message);
            }
        }

        private class UserItem
        {
            public string Id { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public override string ToString() => Name;
        }

        private async void btnTrending_Click(object sender, EventArgs e)
        {
            var docs = await _reportService.GetTrendingAsync();
            BindAggregationResult(docs);
        }

        private async void btnPending_Click(object sender, EventArgs e)
        {
            var docs = await _reportService.GetPendingAsync();
            BindAggregationResult(docs);
        }

        private async void btnContributors_Click(object sender, EventArgs e)
        {
            var docs = await _reportService.GetContributorsAsync();
            BindAggregationResult(docs);
        }

        private async void btnSections_Click(object sender, EventArgs e)
        {
            var docs = await _reportService.GetSectionsAsync();
            BindAggregationResult(docs);
        }

        private void btnOpenInsert_Click(object sender, EventArgs e)
        {
            using (var form = new InsertMaterialForm(_context))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    LoadApprovedMaterials();
                }
            }
        }

        private void btnOpenUpdate_Click(object sender, EventArgs e)
        {
            if (UserSession.CurrentUserRole.ToLower() == "student")
            {
                MessageBox.Show("Students do not have permission to update materials.", "Access Denied");
                return;
            }

            using (var form = new UpdateMaterialForm(_context))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    LoadApprovedMaterials();
                }
            }
        }

        private void btnOpenDelete_Click(object sender, EventArgs e)
        {
            if (UserSession.CurrentUserRole.ToLower() == "student")
            {
                MessageBox.Show("Students can only delete their own materials.", "Access Denied");
                return;
            }

            using (var form = new DeleteMaterialForm(_context))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    LoadApprovedMaterials();
                }
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            UserSession.Logout();
            var login = new LoginForm(_context);
            login.FormClosed += (s, _) => this.Close();
            this.Hide();
            login.Show();
        }

        private void btnOpenAddSection_Click(object sender, EventArgs e)
        {
            if (UserSession.CurrentUserRole != "admin")
            {
                MessageBox.Show("Access Denied. Only administrators can add sections.", "Security Restriction");
                return;
            }
            using (var addSectionForm = new AddSectionForm(_context))
            {
                var result = addSectionForm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    btnSections_Click(this, EventArgs.Empty);
                }
            }
        }

        private void btnOpenAddCourse_Click(object sender, EventArgs e)
        {
            if (UserSession.CurrentUserRole != "admin")
            {
                MessageBox.Show("Access Denied. Only administrators can add courses.", "Security Restriction");
                return;
            }

            using (var addCourseForm = new CourseSharesApp.Forms.Courses.AddCourseForm(_context))
            {
                var result = addCourseForm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    try { btnSections_Click(this, EventArgs.Empty); } catch { }
                }
            }
        }
    }
}