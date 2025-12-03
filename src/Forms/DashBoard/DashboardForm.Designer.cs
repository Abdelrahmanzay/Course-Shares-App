namespace CourseSharesApp.Forms
{
    partial class DashboardForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dgvResults;
        private System.Windows.Forms.Button btnTrending;
        private System.Windows.Forms.Button btnPending;
        private System.Windows.Forms.Button btnContributors;
        private System.Windows.Forms.Button btnSections;
        private System.Windows.Forms.Button btnOpenInsert;
        private System.Windows.Forms.Button btnOpenUpdate;
        private System.Windows.Forms.Button btnOpenDelete;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Panel panelMenu;
        private System.Windows.Forms.Button btnHome;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnSearch;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

      private void InitializeComponent()
{
    // Sidebar colors
    System.Drawing.Color menuColor = System.Drawing.Color.FromArgb(41, 53, 65);
    System.Drawing.Color buttonColor = System.Drawing.Color.FromArgb(51, 63, 75);
    System.Drawing.Color hoverColor = System.Drawing.Color.FromArgb(255, 193, 7);

    this.dgvResults = new System.Windows.Forms.DataGridView();
    this.btnTrending = new System.Windows.Forms.Button();
    this.btnPending = new System.Windows.Forms.Button();
    this.btnContributors = new System.Windows.Forms.Button();
    this.btnSections = new System.Windows.Forms.Button();
    this.btnOpenInsert = new System.Windows.Forms.Button();
    this.btnOpenUpdate = new System.Windows.Forms.Button();
    this.btnOpenDelete = new System.Windows.Forms.Button();
    this.btnLogout = new System.Windows.Forms.Button();
    this.panelMenu = new System.Windows.Forms.Panel();
    this.btnHome = new System.Windows.Forms.Button();
    this.txtSearch = new System.Windows.Forms.TextBox();
    this.btnSearch = new System.Windows.Forms.Button();

    ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).BeginInit();
    this.panelMenu.SuspendLayout();
    this.SuspendLayout();

    // -------------------------
    // Sidebar panel
    // -------------------------
    this.panelMenu.Dock = System.Windows.Forms.DockStyle.Left;
    this.panelMenu.Width = 200;
    this.panelMenu.BackColor = menuColor;
    this.panelMenu.Padding = new System.Windows.Forms.Padding(10, 20, 10, 10);

    // -------------------------
    // Button settings for reports
    // -------------------------
    System.Windows.Forms.Button[] reportButtons =
        { this.btnTrending, this.btnPending, this.btnContributors, this.btnSections };

    foreach (var btn in reportButtons)
    {
        btn.BackColor = buttonColor;
        btn.ForeColor = System.Drawing.Color.White;
        btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        btn.FlatAppearance.BorderSize = 0;
        btn.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
        btn.Dock = System.Windows.Forms.DockStyle.Top;
        btn.Height = 50;
        btn.Margin = new System.Windows.Forms.Padding(0, 0, 0, 5);
    }

    this.btnTrending.Text = "📈 Trending Materials";
    this.btnPending.Text = "🔔 Pending Approval";
    this.btnContributors.Text = "🏆 Top Contributors";
    this.btnSections.Text = "📚 Section Activity";

    // 🔥 ADD CLICK EVENTS
    this.btnTrending.Click += new System.EventHandler(this.btnTrending_Click);
    this.btnPending.Click += new System.EventHandler(this.btnPending_Click);
    this.btnContributors.Click += new System.EventHandler(this.btnContributors_Click);
    this.btnSections.Click += new System.EventHandler(this.btnSections_Click);

    // -------------------------
    // Home Button
    // -------------------------
    this.btnHome.BackColor = buttonColor;
    this.btnHome.ForeColor = System.Drawing.Color.White;
    this.btnHome.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
    this.btnHome.FlatAppearance.BorderSize = 0;
    this.btnHome.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
    this.btnHome.Dock = System.Windows.Forms.DockStyle.Top;
    this.btnHome.Height = 50;
    this.btnHome.Text = "🏠 Home";
    this.btnHome.Click += new System.EventHandler(this.btnHome_Click);

    // -------------------------
    // CRUD Buttons
    // -------------------------
    System.Windows.Forms.Button[] crudButtons =
        { this.btnOpenInsert, this.btnOpenUpdate, this.btnOpenDelete };

    foreach (var btn in crudButtons)
    {
        btn.BackColor = hoverColor;
        btn.ForeColor = System.Drawing.Color.Black;
        btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        btn.FlatAppearance.BorderSize = 0;
        btn.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
        btn.Dock = System.Windows.Forms.DockStyle.Bottom;
        btn.Height = 40;
    }

    this.btnOpenInsert.Text = "➕ Upload Material";
    this.btnOpenUpdate.Text = "✅ Approve / Update";
    this.btnOpenDelete.Text = "🗑 Delete Material";

    // 🔥 ADD CLICK EVENTS
    this.btnOpenInsert.Click += new System.EventHandler(this.btnOpenInsert_Click);
    this.btnOpenUpdate.Click += new System.EventHandler(this.btnOpenUpdate_Click);
    this.btnOpenDelete.Click += new System.EventHandler(this.btnOpenDelete_Click);

    // -------------------------
    // LOGOUT BUTTON
    // -------------------------
    this.btnLogout.BackColor = System.Drawing.Color.FromArgb(220, 80, 80);
    this.btnLogout.ForeColor = System.Drawing.Color.White;
    this.btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
    this.btnLogout.FlatAppearance.BorderSize = 0;
    this.btnLogout.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
    this.btnLogout.Dock = System.Windows.Forms.DockStyle.Bottom;
    this.btnLogout.Height = 40;
    this.btnLogout.Text = "🚪 Logout";
    this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);

    // Add everything to sidebar
    this.panelMenu.Controls.Add(this.btnLogout);
    this.panelMenu.Controls.Add(this.btnOpenDelete);
    this.panelMenu.Controls.Add(this.btnOpenUpdate);
    this.panelMenu.Controls.Add(this.btnOpenInsert);
    this.panelMenu.Controls.Add(this.btnSections);
    this.panelMenu.Controls.Add(this.btnContributors);
    this.panelMenu.Controls.Add(this.btnPending);
    this.panelMenu.Controls.Add(this.btnTrending);
    this.panelMenu.Controls.Add(this.btnHome);

    // -------------------------
    // Search box
    // -------------------------
    this.txtSearch.Location = new System.Drawing.Point(220, 12);
    this.txtSearch.Width = 400;
    this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 10F);
    this.txtSearch.PlaceholderText = "Search Materials...";

    this.btnSearch.Location = new System.Drawing.Point(630, 12);
    this.btnSearch.Size = new System.Drawing.Size(100, 30);
    this.btnSearch.Text = "🔍 Search";
    this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);

    // -------------------------
    // DataGrid
    // -------------------------
    this.dgvResults.Location = new System.Drawing.Point(220, 50);
    this.dgvResults.Size = new System.Drawing.Size(560, 382);
    this.dgvResults.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
    this.dgvResults.RowHeadersVisible = false;

    // -------------------------
    // Dashboard Form
    // -------------------------
    this.ClientSize = new System.Drawing.Size(800, 450);
    this.Controls.Add(this.dgvResults);
    this.Controls.Add(this.txtSearch);
    this.Controls.Add(this.btnSearch);
    this.Controls.Add(this.panelMenu);
    this.Text = "Course Shares Dashboard";

    ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).EndInit();
    this.panelMenu.ResumeLayout(false);
    this.ResumeLayout(false);
}
    }
}