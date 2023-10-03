using Notification.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.UI.Notifications;

namespace Notification
{
    internal class CommandBuilder
    {
        public static Windows.Foundation.TypedEventHandler<ToastNotification, object> BuildCommand(CommandModel commandModel)
        {            
            switch (commandModel.Type)
            {
                case "link": return (s, e) => {
                    OpenLink(commandModel.Data);
                };

                default: return (s, e) => { Console.WriteLine("Unknown command type"); };
            }
        }

        public static void OpenLink(string link) {
            try
            {
                var uri = link;
                var psi = new System.Diagnostics.ProcessStartInfo();
                psi.UseShellExecute = true;
                psi.FileName = uri;
                System.Diagnostics.Process.Start(psi);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        public static void RunApp(string path, string[] args = null)
        {
            // Prepare the process to run
            ProcessStartInfo start = new ProcessStartInfo();
            // Enter in the command line arguments, everything you would enter after the executable name itself
            if (args != null && args.Length>0) { start.Arguments = String.Join(" ", args); }
            // Enter the executable to run, including the complete path
            start.FileName = path;
            // Do you want to show a console window?
            //start.WindowStyle = ProcessWindowStyle.Hidden;
            //start.CreateNoWindow = true;
            int exitCode;
            Process.Start(start);

            //// Run the external process & wait for it to finish
            //using (Process proc = Process.Start(start))
            //{
            //    proc.WaitForExit();

            //    // Retrieve the app's exit code
            //    exitCode = proc.ExitCode;
            //}
        }
    }
}
