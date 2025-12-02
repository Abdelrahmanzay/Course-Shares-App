using System;
using System.Windows.Forms;
using CourseSharesApp.Data;
using CourseSharesApp.Forms;

namespace CourseSharesApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var context = new DatabaseContext();
            Application.Run(new DashboardForm(context));
        }
    }
}