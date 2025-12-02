namespace CourseSharesApp.Forms
{
    partial class UpdateForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtSearchTitle;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnApprove;
        private System.Windows.Forms.Label lblInfo;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtSearchTitle = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnApprove = new System.Windows.Forms.Button();
            this.lblInfo = new System.Windows.Forms.Label();

            this.txtSearchTitle.Location = new System.Drawing.Point(20, 20);
            this.txtSearchTitle.Size = new System.Drawing.Size(200, 20);
            this.txtSearchTitle.Text = "Enter Title to Search";

            this.btnSearch.Text = "Search";
            this.btnSearch.Location = new System.Drawing.Point(230, 18);
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);

            this.lblInfo.Location = new System.Drawing.Point(20, 60);
            this.lblInfo.Size = new System.Drawing.Size(300, 30);
            this.lblInfo.Text = "Result will appear here...";

            this.btnApprove.Text = "Mark as Approved";
            this.btnApprove.Location = new System.Drawing.Point(20, 100);
            this.btnApprove.Size = new System.Drawing.Size(200, 40);
            this.btnApprove.Enabled = false;
            this.btnApprove.Click += new System.EventHandler(this.btnApprove_Click);

            this.ClientSize = new System.Drawing.Size(350, 180);
            this.Controls.Add(this.txtSearchTitle);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.btnApprove);
            this.Text = "Update Material Status";
        }
    }
}