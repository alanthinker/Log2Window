using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;


namespace Log2Window
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Trace.WriteLine(e.Exception.ToString());

            //MessageBox.Show(e.Exception.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Trace.WriteLine(e.ExceptionObject.ToString());
            //MessageBox.Show(e.ExceptionObject.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}