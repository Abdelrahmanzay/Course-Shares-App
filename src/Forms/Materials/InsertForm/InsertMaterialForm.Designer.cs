namespace CourseSharesApp.Forms.Materials
{
    partial class InsertMaterialForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.RadioButton rbFile;
        private System.Windows.Forms.RadioButton rbLink;
        private System.Windows.Forms.Label lblLink;
        private System.Windows.Forms.TextBox txtLink;
        private System.Windows.Forms.Label lblCourse;
        private System.Windows.Forms.ComboBox cmbCourse;
        private System.Windows.Forms.Button btnSave;
        
        // Added the Browse button declaration
        private System.Windows.Forms.Button btnBrowse; 

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
            this.lblTitle = new System.Windows.Forms.Label();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.lblType = new System.Windows.Forms.Label();
            this.rbFile = new System.Windows.Forms.RadioButton();
            this.rbLink = new System.Windows.Forms.RadioButton();
            this.lblLink = new System.Windows.Forms.Label();
            this.txtLink = new System.Windows.Forms.TextBox();
            this.lblCourse = new System.Windows.Forms.Label();
            this.cmbCourse = new System.Windows.Forms.ComboBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnBrowse = new System.Windows.Forms.Button(); // Initialize Button
            this.SuspendLayout();

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(30, 13);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Title:";

            // txtTitle
            this.txtTitle.Location = new System.Drawing.Point(100, 17);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(250, 20);
            this.txtTitle.TabIndex = 1;

            // lblType
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(20, 50);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(34, 13);
            this.lblType.TabIndex = 2;
            this.lblType.Text = "Type:";

            // rbFile
            this.rbFile.AutoSize = true;
            this.rbFile.Checked = true;
            this.rbFile.Location = new System.Drawing.Point(100, 48);
            this.rbFile.Name = "rbFile";
            this.rbFile.Size = new System.Drawing.Size(41, 17);
            this.rbFile.TabIndex = 3;
            this.rbFile.TabStop = true;
            this.rbFile.Text = "File";

            // rbLink
            this.rbLink.AutoSize = true;
            this.rbLink.Location = new System.Drawing.Point(160, 48);
            this.rbLink.Name = "rbLink";
            this.rbLink.Size = new System.Drawing.Size(45, 17);
            this.rbLink.TabIndex = 4;
            this.rbLink.Text = "Link";

            // lblLink
            this.lblLink.AutoSize = true;
            this.lblLink.Location = new System.Drawing.Point(20, 80);
            this.lblLink.Name = "lblLink";
            this.lblLink.Size = new System.Drawing.Size(56, 13);
            this.lblLink.TabIndex = 5;
            this.lblLink.Text = "File/Link:";

            // txtLink
            this.txtLink.Location = new System.Drawing.Point(100, 77);
            this.txtLink.Name = "txtLink";
            this.txtLink.Size = new System.Drawing.Size(250, 20);
            this.txtLink.TabIndex = 6;

            // 
            // btnBrowse (New Button Configuration)
            // 
            this.btnBrowse.Location = new System.Drawing.Point(360, 75);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 7;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            // IMPORTANT: This connects the click to the code-behind logic
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);

            // lblCourse
            this.lblCourse.AutoSize = true;
            this.lblCourse.Location = new System.Drawing.Point(20, 110);
            this.lblCourse.Name = "lblCourse";
            this.lblCourse.Size = new System.Drawing.Size(43, 13);
            this.lblCourse.TabIndex = 8;
            this.lblCourse.Text = "Course:";

            // cmbCourse
            this.cmbCourse.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCourse.Location = new System.Drawing.Point(100, 107);
            this.cmbCourse.Name = "cmbCourse";
            this.cmbCourse.Size = new System.Drawing.Size(250, 21);
            this.cmbCourse.TabIndex = 9;

            // btnSave
            this.btnSave.Location = new System.Drawing.Point(150, 150);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 30);
            this.btnSave.TabIndex = 10;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            // InsertMaterialForm
            this.ClientSize = new System.Drawing.Size(450, 210); // Slightly wider to fit Browse button
            this.Controls.Add(this.btnBrowse); // Add Browse button to controls
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.cmbCourse);
            this.Controls.Add(this.lblCourse);
            this.Controls.Add(this.txtLink);
            this.Controls.Add(this.lblLink);
            this.Controls.Add(this.rbLink);
            this.Controls.Add(this.rbFile);
            this.Controls.Add(this.lblType);
            this.Controls.Add(this.txtTitle);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "InsertMaterialForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Insert Material";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}