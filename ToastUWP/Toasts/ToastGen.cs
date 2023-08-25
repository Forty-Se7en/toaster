using Microsoft.Toolkit.Uwp.Notifications;
using Notification.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Foundation.Collections;

namespace ToastUWP.Toasts
{
    internal class ToastGen
    {
        public static ToastContentBuilder GenerateToast(ToastModel toast)
        {
            var builder = new ToastContentBuilder();
            builder.SetBackgroundActivation();
            //builder.AddArgument("SomeArgKey1");
            //builder.AddArgument("SomeArgKey2");
            if (toast.Header != null)
            {
                builder.AddHeader("0", toast.Header, new ToastArguments());
            }
            if (toast.Title != null)
            {
                builder.AddText(toast.Title);
            }
            if (toast.Message != null)
            {
                builder.AddText(toast.Message);
            }
            if (toast.Text != null)
            {
                builder.AddText(toast.Text);
            }
            if (toast.Label != null)            {

                builder.AddAttributionText(toast.Label);
            }
            if (toast.Logo != null)
            {
                builder.AddAppLogoOverride(new Uri(toast.Logo), ToastGenericAppLogoCrop.Circle);
            }
            //builder.AddAttributionText("AttributionText");
            //builder.AddInputTextBox("0", title: "answer");
            //builder.AddHeroImage(new Uri("D:\\img\\картинки тест\\rad.jpg"));
            if (toast.Image != null)
            {
                builder.AddInlineImage(new Uri(toast.Image));
            }
            if (toast.HeroImage != null)
            {
                builder.AddHeroImage(new Uri(toast.HeroImage));
            }

            return builder;

        }

        public static void ExecuteCommand(CommandModel command)
        {
            switch (command.Type)
            {
                case "link":
                    OpenLink(command.Data);
                    break;
                default: Console.WriteLine("Unknown command type"); break;
            }
        }

        static void OpenLink(string link)
        {
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

        //protected override void OnActivated(IActivatedEventArgs e)
        //{
        //    // Handle notification activation
        //    if (e is ToastNotificationActivatedEventArgs toastActivationArgs)
        //    {
        //        // Obtain the arguments from the notification
        //        ToastArguments args = ToastArguments.Parse(toastActivationArgs.Argument);

        //        // Obtain any user input (text boxes, menu selections) from the notification
        //        ValueSet userInput = toastActivationArgs.UserInput;

        //        // TODO: Show the corresponding content
        //    }
        //}
    }
}
