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
            // Define colors for a cohesive look (Adjust these RGB values as needed)
            System.Drawing.Color menuColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(53)))), ((int)(((byte)(65))))); // Dark Blue-Gray
            System.Drawing.Color buttonColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(63)))), ((int)(((byte)(75))))); // Slightly Lighter Menu Button
            System.Drawing.Color hoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(193)))), ((int)(((byte)(7))))); // Yellow/Gold accent for hover/active

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
            
            // 
            // panelMenu (Left Sidebar Styling)
            // 
            this.panelMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelMenu.Width = 200;
            this.panelMenu.BackColor = menuColor; // Dark background for the menu
            this.panelMenu.Controls.Add(this.btnOpenDelete);
            this.panelMenu.Controls.Add(this.btnOpenUpdate);
            this.panelMenu.Controls.Add(this.btnOpenInsert);
            this.panelMenu.Controls.Add(this.btnSections);
            this.panelMenu.Controls.Add(this.btnContributors);
            this.panelMenu.Controls.Add(this.btnPending);
            this.panelMenu.Controls.Add(this.btnTrending);
            this.panelMenu.Padding = new System.Windows.Forms.Padding(10, 20, 10, 10);
            
            // 
            // Button Styles (Aggregation Reports)
            // 
            System.Windows.Forms.Button[] reportButtons = { this.btnTrending, this.btnPending, this.btnContributors, this.btnSections };
            foreach (var btn in reportButtons)
            {
                btn.BackColor = buttonColor;
                btn.ForeColor = System.Drawing.Color.White;
                btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
                btn.Dock = System.Windows.Forms.DockStyle.Top;
                btn.Height = 50;
                btn.Margin = new System.Windows.Forms.Padding(0, 0, 0, 5); // Spacing between buttons
            }
            
            this.btnTrending.Text = "📈 Trending Materials Report";
            this.btnTrending.Click += new System.EventHandler(this.btnTrending_Click);
            
            this.btnPending.Text = "🔔 Pending Approval Queue";
            this.btnPending.Click += new System.EventHandler(this.btnPending_Click);
            
            this.btnContributors.Text = "🏆 Top Contributors";
            this.btnContributors.Click += new System.EventHandler(this.btnContributors_Click);
            
            this.btnSections.Text = "📚 Section Activity Overview";
            this.btnSections.Click += new System.EventHandler(this.btnSections_Click);

            // 
            // Button Styles (CRUD Operations)
            // 
            System.Windows.Forms.Button[] crudButtons = { this.btnOpenInsert, this.btnOpenUpdate, this.btnOpenDelete };
            foreach (var btn in crudButtons)
            {
                btn.BackColor = hoverColor; // Highlight CRUD actions
                btn.ForeColor = System.Drawing.Color.Black;
                btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
                btn.Dock = System.Windows.Forms.DockStyle.Bottom;
                btn.Height = 40;
                btn.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0); 
            }
            
            this.btnOpenInsert.Text = "➕ Upload New Material";
            this.btnOpenInsert.Click += new System.EventHandler(this.btnOpenInsert_Click);
            
            this.btnOpenUpdate.Text = "✅ Approve / Update Status";
            this.btnOpenUpdate.Click += new System.EventHandler(this.btnOpenUpdate_Click);
            
            this.btnOpenDelete.Text = "🗑️ Delete Material";
            this.btnOpenDelete.Click += new System.EventHandler(this.btnOpenDelete_Click);

            // 
            // dgvResults (Data Grid Styling)
            // 
            this.dgvResults.Location = new System.Drawing.Point(220, 12);
            this.dgvResults.Size = new System.Drawing.Size(560, 420);
            this.dgvResults.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvResults.BackgroundColor = System.Drawing.Color.WhiteSmoke; // Light background
            this.dgvResults.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvResults.RowHeadersVisible = false;
            this.dgvResults.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.dgvResults.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;


            // 
            // DashboardForm (Main Form Styling)
            // 
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dgvResults);
            this.Controls.Add(this.panelMenu);
            this.Text = "Course Shares Dashboard"; // This text is visible in the Windows bar
            this.BackColor = System.Drawing.Color.White;
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).EndInit();
            this.panelMenu.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}