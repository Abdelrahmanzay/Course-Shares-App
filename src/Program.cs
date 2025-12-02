using System;
using System.Windows.Forms;
using CourseSharesApp.Data;
using CourseSharesApp.Forms.Auth;

namespace CourseSharesApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new LoginForm());
        }
    }
}