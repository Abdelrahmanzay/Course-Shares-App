using System;
using System.Windows.Forms;
using CourseSharesApp.Data;
using CourseSharesApp.Forms;

namespace CourseSharesApp
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var ctx = new DatabaseContext("mongodb+srv://Agent:CyO41ftEO2jYc3Jf@agents.jfuv468.mongodb.net/");
            if (!ctx.CanConnect())
            {
                MessageBox.Show("Cannot connect to MongoDB. Please check network and credentials.");
                return;
            }

            Application.Run(new DashboardForm(ctx));
        }
    }
}
