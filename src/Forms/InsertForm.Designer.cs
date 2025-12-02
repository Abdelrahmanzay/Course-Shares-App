namespace CourseSharesApp.Forms
{
    partial class InsertForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.TextBox txtLink;
        private System.Windows.Forms.ComboBox cmbCourse;
        private System.Windows.Forms.RadioButton rbFile;
        private System.Windows.Forms.RadioButton rbLink;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblLink;
        private System.Windows.Forms.Label lblCourse;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.txtLink = new System.Windows.Forms.TextBox();
            this.cmbCourse = new System.Windows.Forms.ComboBox();
            this.rbFile = new System.Windows.Forms.RadioButton();
            this.rbLink = new System.Windows.Forms.RadioButton();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblLink = new System.Windows.Forms.Label();
            this.lblCourse = new System.Windows.Forms.Label();
            
            // Labels
            this.lblTitle.Text = "Title:";
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            
            this.lblCourse.Text = "Course:";
            this.lblCourse.Location = new System.Drawing.Point(20, 60);

            this.lblLink.Text = "Link/Path:";
            this.lblLink.Location = new System.Drawing.Point(20, 140);

            // Controls
            this.txtTitle.Location = new System.Drawing.Point(100, 20);
            this.txtTitle.Size = new System.Drawing.Size(200, 20);

            this.cmbCourse.Location = new System.Drawing.Point(100, 60);
            this.cmbCourse.Size = new System.Drawing.Size(200, 20);

            this.rbFile.Text = "File";
            this.rbFile.Checked = true;
            this.rbFile.Location = new System.Drawing.Point(100, 100);
            
            this.rbLink.Text = "Link";
            this.rbLink.Location = new System.Drawing.Point(160, 100);

            this.txtLink.Location = new System.Drawing.Point(100, 140);
            this.txtLink.Size = new System.Drawing.Size(200, 20);

            this.btnSave.Text = "Insert";
            this.btnSave.Location = new System.Drawing.Point(100, 180);
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            this.ClientSize = new System.Drawing.Size(350, 250);
            this.Controls.Add(this.lblTitle); this.Controls.Add(this.txtTitle);
            this.Controls.Add(this.lblCourse); this.Controls.Add(this.cmbCourse);
            this.Controls.Add(this.rbFile); this.Controls.Add(this.rbLink);
            this.Controls.Add(this.lblLink); this.Controls.Add(this.txtLink);
            this.Controls.Add(this.btnSave);
            this.Text = "Insert Material";
        }
    }
}