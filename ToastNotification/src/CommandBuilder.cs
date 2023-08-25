using Notification.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        static void OpenLink(string link) {
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
    }
}
