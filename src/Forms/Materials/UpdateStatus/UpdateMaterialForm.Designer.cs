namespace CourseSharesApp.Forms.Materials
{
    partial class UpdateMaterialForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearchTitle;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Button btnApprove;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearchTitle = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.lblInfo = new System.Windows.Forms.Label();
            this.btnApprove = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // lblSearch
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(20, 20);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(75, 13);
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Text = "Material Title:";

            // txtSearchTitle
            this.txtSearchTitle.Location = new System.Drawing.Point(100, 17);
            this.txtSearchTitle.Name = "txtSearchTitle";
            this.txtSearchTitle.Size = new System.Drawing.Size(250, 20);
            this.txtSearchTitle.TabIndex = 1;

            // btnSearch
            this.btnSearch.Location = new System.Drawing.Point(150, 50);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(100, 30);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "Search";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);

            // lblInfo
            this.lblInfo.Location = new System.Drawing.Point(20, 100);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(360, 40);
            this.lblInfo.TabIndex = 3;
            this.lblInfo.Text = "Search for a material to approve.";

            // btnApprove
            this.btnApprove.Enabled = false;
            this.btnApprove.Location = new System.Drawing.Point(150, 150);
            this.btnApprove.Name = "btnApprove";
            this.btnApprove.Size = new System.Drawing.Size(100, 30);
            this.btnApprove.TabIndex = 4;
            this.btnApprove.Text = "Approve";
            this.btnApprove.Click += new System.EventHandler(this.btnApprove_Click);

            // UpdateMaterialForm
            this.ClientSize = new System.Drawing.Size(400, 210);
            this.Controls.Add(this.btnApprove);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.txtSearchTitle);
            this.Controls.Add(this.lblSearch);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "UpdateMaterialForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Update Material Status";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
