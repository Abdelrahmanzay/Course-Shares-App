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
        private System.Windows.Forms.Panel panelMenu;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.dgvResults = new System.Windows.Forms.DataGridView();
            this.btnTrending = new System.Windows.Forms.Button();
            this.btnPending = new System.Windows.Forms.Button();
            this.btnContributors = new System.Windows.Forms.Button();
            this.btnSections = new System.Windows.Forms.Button();
            this.btnOpenInsert = new System.Windows.Forms.Button();
            this.btnOpenUpdate = new System.Windows.Forms.Button();
            this.btnOpenDelete = new System.Windows.Forms.Button();
            this.panelMenu = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).BeginInit();
            this.panelMenu.SuspendLayout();
            this.SuspendLayout();
            
            // Grid
            this.dgvResults.Location = new System.Drawing.Point(220, 12);
            this.dgvResults.Size = new System.Drawing.Size(560, 420);
            this.dgvResults.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            
            // Buttons
            this.panelMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelMenu.Width = 200;
            this.panelMenu.Controls.Add(this.btnOpenDelete);
            this.panelMenu.Controls.Add(this.btnOpenUpdate);
            this.panelMenu.Controls.Add(this.btnOpenInsert);
            this.panelMenu.Controls.Add(this.btnSections);
            this.panelMenu.Controls.Add(this.btnContributors);
            this.panelMenu.Controls.Add(this.btnPending);
            this.panelMenu.Controls.Add(this.btnTrending);

            this.btnTrending.Text = "Trending Report";
            this.btnTrending.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnTrending.Height = 50;
            this.btnTrending.Click += new System.EventHandler(this.btnTrending_Click);

            this.btnPending.Text = "Pending Approvals";
            this.btnPending.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnPending.Height = 50;
            this.btnPending.Click += new System.EventHandler(this.btnPending_Click);

            this.btnContributors.Text = "Top Contributors";
            this.btnContributors.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnContributors.Height = 50;
            this.btnContributors.Click += new System.EventHandler(this.btnContributors_Click);

            this.btnSections.Text = "Section Activity";
            this.btnSections.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnSections.Height = 50;
            this.btnSections.Click += new System.EventHandler(this.btnSections_Click);

            this.btnOpenInsert.Text = "+ Add Material";
            this.btnOpenInsert.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnOpenInsert.Height = 40;
            this.btnOpenInsert.Click += new System.EventHandler(this.btnOpenInsert_Click);

            this.btnOpenUpdate.Text = "Update Status";
            this.btnOpenUpdate.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnOpenUpdate.Height = 40;
            this.btnOpenUpdate.Click += new System.EventHandler(this.btnOpenUpdate_Click);

            this.btnOpenDelete.Text = "Delete Material";
            this.btnOpenDelete.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnOpenDelete.Height = 40;
            this.btnOpenDelete.Click += new System.EventHandler(this.btnOpenDelete_Click);

            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dgvResults);
            this.Controls.Add(this.panelMenu);
            this.Text = "Course Shares Dashboard";
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).EndInit();
            this.panelMenu.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}