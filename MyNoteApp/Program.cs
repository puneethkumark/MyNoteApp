using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
namespace MyNoteApp
{
    static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                //configure for simulator app 
                if (0 == string.Compare(args[0].ToString(), "cloud", true))
                {
                    ConfigureCloudApp();
                }
            }

            string applicationMutexName;
            if (Common.DesktopApplicaiton)
            {
                applicationMutexName = "MyNoteApp_Desktop";
            }
            else
            {
                applicationMutexName = "MyNoteApp_Cloud";
            }


            using (Mutex mutex = new Mutex(false, applicationMutexName))
            {
                if (!mutex.WaitOne(0, true))
                {
                    MessageBox.Show(applicationMutexName + " is already runnig. Unable to run multiple instances of this program.",
                                    "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
                else
                {   
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new MyNote());
                }
            }
        }

        static void ConfigureCloudApp()
        {
            Common.DesktopApplicaiton = false;
            Common.MyNoteAppConfigFile = Common.MyNoteAppConfigCloudFile;
            Common.DefaultConfigString = Common.DefaultCloudConfigString;
            Common.LogFileName = Common.LogFileName_cloud;
        }
    }
}
