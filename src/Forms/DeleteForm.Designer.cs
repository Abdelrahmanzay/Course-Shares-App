namespace CourseSharesApp.Forms
{
    partial class DeleteForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Label lblInstruction;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.lblInstruction = new System.Windows.Forms.Label();

            this.lblInstruction.Text = "Enter Title to Delete:";
            this.lblInstruction.Location = new System.Drawing.Point(20, 20);
            this.lblInstruction.AutoSize = true;

            this.txtTitle.Location = new System.Drawing.Point(20, 50);
            this.txtTitle.Size = new System.Drawing.Size(200, 20);

            this.btnDelete.Text = "DELETE";
            this.btnDelete.ForeColor = System.Drawing.Color.Red;
            this.btnDelete.Location = new System.Drawing.Point(20, 90);
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);

            this.ClientSize = new System.Drawing.Size(250, 150);
            this.Controls.Add(this.lblInstruction);
            this.Controls.Add(this.txtTitle);
            this.Controls.Add(this.btnDelete);
            this.Text = "Delete Material";
        }
    }
}