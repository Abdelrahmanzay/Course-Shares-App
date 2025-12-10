using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using CourseSharesApp.Data;
using CourseSharesApp.Forms.Materials;
using CourseSharesApp.Forms.Auth;
using CourseSharesApp.Auth;
using CourseSharesApp.Forms.Sections;

namespace CourseSharesApp.Forms
{
    public partial class DashboardForm : Form
    {
        private readonly DatabaseContext _context;

        public DashboardForm(DatabaseContext context)
        {
            InitializeComponent();
            _context = context;


            if (UserSession.CurrentUserRole.ToLower() == "student")
        {
            btnOpenUpdate.Visible = false;
            btnOpenDelete.Visible = false;
            btnPending.Visible = false;
            
            // NEW: Hide Add Section button for students
            try { btnOpenAddSection.Visible = false; } catch { /* Ignore if button wasn't defined */ }
        }
        
        // NEW: Check if the user is NOT an admin, and hide the button if they aren't.
        if (UserSession.CurrentUserRole.ToLower() != "admin")
        {
            try { btnOpenAddSection.Visible = false; } catch { /* Ignore if button wasn't defined */ }
        }

            // Hide buttons for students
            if (UserSession.CurrentUserRole.ToLower() == "student")
            {
                btnOpenUpdate.Visible = false;
                btnOpenDelete.Visible = false;
                btnPending.Visible = false;
            }

            // Prepare user dropdown (hidden by default) and wire handler
            try
            {
                cmbUserList.Visible = false;
                cmbUserList.DisplayMember = "Name";
                cmbUserList.ValueMember = "Id";
                cmbUserList.SelectedIndexChanged -= cmbUserList_SelectedIndexChanged;
                cmbUserList.SelectedIndexChanged += cmbUserList_SelectedIndexChanged;

                // position the combobox under the search textbox if available
                cmbUserList.Location = new Point(txtSearch.Location.X, txtSearch.Location.Y + txtSearch.Height + 6);
                cmbUserList.Width = Math.Max(220, txtSearch.Width + 40);
            }
            catch
            {
                // ignore designer differences
            }

            // Load approved materials on start
            LoadApprovedMaterials();

            // Add handler for hyperlink clicks
            dgvResults.CellContentClick += DgvResults_CellContentClick;
        }

        // ------------------- LOAD MATERIALS -------------------
        private async void LoadApprovedMaterials()
        {
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$match", new BsonDocument("status", "Approved")),
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "users" },
                    { "localField", "uploadedBy" },
                    { "foreignField", "_id" },
                    { "as", "uploader" }
                }),
                new BsonDocument("$unwind", "$uploader"),
                new BsonDocument("$project", new BsonDocument
                {
                    { "Title", "$title" },
                    { "UploadedBy", "$uploader.name" },
                    { "Status", "$status" },
                    { "UploadDate", "$uploadDate" },
                    { "ViewsCount", "$viewsCount" },
                    { "FileLink", "$fileLink" }, // Added hyperlink column
                    { "_id", "$_id" }
                })
            };

            await RunAggregation("materials", pipeline);
        }

        // ------------------- AGGREGATION HELPER -------------------
        private async System.Threading.Tasks.Task RunAggregation(string collectionName, BsonDocument[] pipeline)
        {
            try
            {
                // Ensure user-list combobox is hidden for general views
                try { cmbUserList.Visible = false; } catch { }

                var collection = _context.Materials.Database.GetCollection<BsonDocument>(collectionName);
                var result = await collection.Aggregate<BsonDocument>(pipeline).ToListAsync();

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

                    // Convert FileLink column to hyperlink
                    if (dt.Columns.Contains("FileLink"))
                    {
                        if (!(dgvResults.Columns["FileLink"] is DataGridViewLinkColumn))
                        {
                            dgvResults.Columns.Remove("FileLink");
                            var linkColumn = new DataGridViewLinkColumn
                            {
                                Name = "FileLink",
                                HeaderText = "File",
                                DataPropertyName = "FileLink",
                                TrackVisitedState = true,
                                LinkColor = System.Drawing.Color.Blue,
                                ActiveLinkColor = System.Drawing.Color.Red,
                                VisitedLinkColor = System.Drawing.Color.Purple
                            };
                            dgvResults.Columns.Add(linkColumn);
                        }
                        // hide the _id column if present
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
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // ------------------- HANDLE HYPERLINK CLICK -------------------
        private async void DgvResults_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvResults.Columns[e.ColumnIndex] is DataGridViewLinkColumn)
            {
                var link = dgvResults.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();
                if (string.IsNullOrEmpty(link)) return;

                // Attempt to increment viewsCount for the material if we have its _id in the row
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
                            // fallback: try matching on string _id or fileLink
                            var filter = Builders<BsonDocument>.Filter.Eq("_id", idStr);
                            var update = Builders<BsonDocument>.Update.Inc("viewsCount", 1);
                            upd = await materialsColl.UpdateOneAsync(filter, update);

                            if (upd?.ModifiedCount == 0)
                            {
                                // try matching by fileLink
                                var linkFilter = Builders<BsonDocument>.Filter.Eq("fileLink", link);
                                upd = await materialsColl.UpdateOneAsync(linkFilter, Builders<BsonDocument>.Update.Inc("viewsCount", 1));
                            }
                        }

                        if (upd != null && upd.ModifiedCount > 0)
                        {
                            // update grid cell if ViewsCount or Views present
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
                        else
                        {
                            // Non blocking diagnostic - developer can remove later
                            Debug.WriteLine("viewsCount not incremented (no matching document).");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // swallow but log to help debugging
                    Debug.WriteLine($"viewsCount update failed: {ex.Message}");
                }

                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = link,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Cannot open file: {ex.Message}");
                }
            }
        }

        // ------------------- BUTTON FUNCTIONALITIES -------------------
        private void btnHome_Click(object sender, EventArgs e) => LoadApprovedMaterials();
        
        private async void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();

            if (string.IsNullOrEmpty(searchTerm))
            {
                LoadApprovedMaterials();
                return;
            }

    // CONTAINS search (matches anywhere in title)
    var regex = new BsonRegularExpression(searchTerm, "i"); // "i" = ignore case

    var pipeline = new BsonDocument[]
    {
        new BsonDocument("$match", new BsonDocument
        {
            { "status", "Approved" },
            { "title", regex }   // <-- REGEX contains search
        }),
        new BsonDocument("$lookup", new BsonDocument
        {
            { "from", "users" },
            { "localField", "uploadedBy" },
            { "foreignField", "_id" },
            { "as", "uploader" }
        }),
        new BsonDocument("$unwind", "$uploader"),
        new BsonDocument("$project", new BsonDocument
        {
            { "Title", "$title" },
            { "UploadedBy", "$uploader.name" },
            { "Status", "$status" },
            { "UploadDate", "$uploadDate" },
            { "ViewsCount", "$viewsCount" },
            { "FileLink", "$fileLink" },
                    { "_id", "$_id" }
        })
    };

    await RunAggregation("materials", pipeline);
}

private async void btnViewUserUploads_Click(object sender, EventArgs e)
        {
            try
            {
                // Collect user identifiers from materials robustly
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
                catch
                {
                    // ignore - fallback to BSON distinct
                }

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

                // Resolve names from users collection where possible
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

                // Position under the search box and show dropdown
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

                // Ensure the data grid is moved down so the combobox dropdown is not overlapped
                try
                {
                    int desiredTop = cmbUserList.Location.Y + cmbUserList.Height + 6;
                    int oldTop = dgvResults.Top;
                    if (oldTop < desiredTop)
                    {
                        int delta = desiredTop - oldTop;
                        dgvResults.Top = desiredTop;
                        // reduce height so bottom stays roughly same, but keep a minimum height
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

        // -----------------------------
        // NEW: Load materials of selected user
        // -----------------------------
        private async void cmbUserList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbUserList.SelectedItem == null) return;
                if (!(cmbUserList.SelectedItem is UserItem selected)) return;

                var materialsColl = _context.Materials.Database.GetCollection<BsonDocument>("materials");

                var filters = new List<FilterDefinition<BsonDocument>>();

                // uploadedBy stored as ObjectId
                if (ObjectId.TryParse(selected.Id, out var parsedOid))
                {
                    filters.Add(Builders<BsonDocument>.Filter.Eq("uploadedBy", parsedOid));
                }

                // possible stored user id string fields
                filters.Add(Builders<BsonDocument>.Filter.Eq("userId", selected.Id));
                filters.Add(Builders<BsonDocument>.Filter.Eq("UserId", selected.Id));

                var combined = Builders<BsonDocument>.Filter.Or(filters);

                var docs = await materialsColl.Find(combined).ToListAsync();

                if (docs == null || docs.Count == 0)
                {
                    dgvResults.DataSource = null;
                    MessageBox.Show("This user has no uploaded materials.");
                    return;
                }

                // Build a minimal DataTable with only Title, UploadedBy (name), FileLink and hidden _id
                var dt = new System.Data.DataTable();
                dt.Columns.Add("Title");
                dt.Columns.Add("UploadedBy");
                dt.Columns.Add("FileLink");
                dt.Columns.Add("_id"); // hidden id used to increment views

                // Fetch uploader name from users collection if not embedded in material doc
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

                // Replace FileLink with a link column
                if (dt.Columns.Contains("FileLink"))
                {
                    // remove existing display column if present
                    if (dgvResults.Columns.Contains("FileLink")) dgvResults.Columns.Remove("FileLink");

                    var linkColumn = new DataGridViewLinkColumn
                    {
                        Name = "FileLink",
                        HeaderText = "File",
                        DataPropertyName = "FileLink",
                        TrackVisitedState = true,
                        LinkColor = System.Drawing.Color.Blue,
                        ActiveLinkColor = System.Drawing.Color.Red,
                        VisitedLinkColor = System.Drawing.Color.Purple
                    };
                    dgvResults.Columns.Add(linkColumn);
                    // hide the _id column if present
                    if (dgvResults.Columns.Contains("_id")) dgvResults.Columns["_id"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading materials: " + ex.Message);
            }
        }

        // Helper: convert list of BsonDocument to DataTable
        private System.Data.DataTable BsonDocumentsToDataTable(System.Collections.Generic.List<BsonDocument> docs)
        {
            var dt = new System.Data.DataTable();

            if (docs == null || docs.Count == 0) return dt;

            var allKeys = new HashSet<string>();
            foreach (var d in docs) foreach (var k in d.Names) allKeys.Add(k);

            foreach (var key in allKeys) dt.Columns.Add(key);

            foreach (var d in docs)
            {
                var row = dt.NewRow();
                foreach (var key in allKeys)
                {
                    if (d.Contains(key))
                    {
                        var val = d[key];
                        if (val.IsObjectId) row[key] = val.AsObjectId.ToString();
                        else if (val.IsBsonDateTime) row[key] = val.ToUniversalTime().ToString("u");
                        else row[key] = val.ToString();
                    }
                    else row[key] = "";
                }
                dt.Rows.Add(row);
            }

            return dt;
        }

        // Simple DTO for combobox binding
        private class UserItem
        {
            public string Id { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public override string ToString() => Name;
        }


        private async void btnTrending_Click(object sender, EventArgs e)
        {
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$match", new BsonDocument("status", "Approved")),
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
                    { "FileLink", "$fileLink" }, // Optional if you want hyperlink here
                    { "_id", "$_id" }
                })
            };

            await RunAggregation("materials", pipeline);
        }

        private async void btnPending_Click(object sender, EventArgs e)
        {
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$match", new BsonDocument("status", "Pending")),
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "users" },
                    { "localField", "uploadedBy" },
                    { "foreignField", "_id" },
                    { "as", "uploader" }
                }),
                new BsonDocument("$unwind", "$uploader"),
                new BsonDocument("$project", new BsonDocument
                {
                    { "Material", "$title" },
                    { "UploadedAt", "$uploadDate" },
                    { "By", "$uploader.name" },
                        { "FileLink", "$fileLink" }, // Optional for pending
                        { "_id", "$_id" }
                })
            };

            await RunAggregation("materials", pipeline);
        }

        private async void btnContributors_Click(object sender, EventArgs e)
        {
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", "$uploadedBy" },
                    { "UploadCount", new BsonDocument("$sum", 1) }
                }),
                new BsonDocument("$sort", new BsonDocument("UploadCount", -1)),
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "users" },
                    { "localField", "_id" },
                    { "foreignField", "_id" },
                    { "as", "user" }
                }),
                new BsonDocument("$unwind", "$user"),
                new BsonDocument("$project", new BsonDocument
                {
                    { "Name", "$user.name" },
                    { "Uploads", "$UploadCount" },
                    { "_id", 0 }
                })
            };

            await RunAggregation("materials", pipeline);
        }

        private async void btnSections_Click(object sender, EventArgs e)
        {
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", "$sectionId" },
                    { "CourseCount", new BsonDocument("$sum", 1) }
                }),
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "sections" },
                    { "localField", "_id" },
                    { "foreignField", "_id" },
                    { "as", "section" }
                }),
                new BsonDocument("$unwind", "$section"),
                new BsonDocument("$project", new BsonDocument
                {
                    { "Section", "$section.sectionName" },
                    { "TotalCourses", "$CourseCount" },
                    { "_id", 0 }
                })
            };

            await RunAggregation("courses", pipeline);
        }

        // ------------------- INSERT / UPDATE / DELETE -------------------
        private void btnOpenInsert_Click(object sender, EventArgs e)
        {
            new InsertMaterialForm(_context).ShowDialog();
        }

        private void btnOpenUpdate_Click(object sender, EventArgs e)
        {
            if (UserSession.CurrentUserRole.ToLower() == "student")
            {
                MessageBox.Show("Students do not have permission to update materials.", "Access Denied");
                return;
            }

            new UpdateMaterialForm(_context).ShowDialog();
        }

        private void btnOpenDelete_Click(object sender, EventArgs e)
        {
            if (UserSession.CurrentUserRole.ToLower() == "student")
            {
                MessageBox.Show("Students can only delete their own materials.", "Access Denied");
                return;
            }

            new DeleteMaterialForm(_context).ShowDialog();
        }
   
        // ------------------- VIEW USER UPLOADS -------------------

        // ------------------- LOGOUT -------------------
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
    // The security check: Only allow Admin role to proceed [cite: 57]
    if (UserSession.CurrentUserRole != "admin")
    {
        MessageBox.Show("Access Denied. Only administrators can add sections.", "Security Restriction");
        return;
    }
    var addSectionForm = new AddSectionForm(_context); 
    addSectionForm.ShowDialog();
    
    // Optional: Refresh the Sections view after adding a new section
    btnSections_Click(this, EventArgs.Empty);

    
}
    }
}