using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace RxMediaPharma
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string name= Process.GetCurrentProcess().ProcessName;
            Process[] ByName = Process.GetProcessesByName(name);
            if (ByName.Length > 1)
            {
                MessageBox.Show("Programın bir örneği çalışmaktadır. Başka bir örnek çalıştıramazsınız");
                return;
            }




            Application.Run(new Form1());
        }
    }
}
