using Newtonsoft.Json;
using Notification.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using Windows.Data.Xml.Dom;
using Windows.UI.Composition;
using Windows.UI.Notifications;
using static System.Collections.Specialized.BitVector32;
using static System.Net.Mime.MediaTypeNames;
using Windows.UI.Popups;

namespace Notification
{
    internal class ToastBuilder
    {
        //private const String APP_ID = "{D65231B0-B2F1-4857-A4CE-A8E7C6EA7D27}\\WindowsPowerShell\\v1.0\\powershell.exe"; 
        //private const String APP_ID = "{D65231B0-B2F1-4857-A4CE-A8E7C6EA7D27}\\cmd.exe";
        //private const String APP_ID = "C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe";
        //private static String APP_ID = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.SystemX86), "\\WindowsPowerShell\\v1.0\\powershell.exe");
        private static String APP_ID = "toaster";

        private static readonly string Separator = "&amp;";

        public static Windows.UI.Notifications.ToastNotification ShowToastFromTemplate(ToastModel toastData)
        {
            //if (toastData.Message == null) return null;
            
            // Get a toast XML template
            XmlDocument toastXml;

            if (toastData.Xml == null)
            {

                //// Specify the absolute path to an image
                //String imagePath = "file:///" + Path.GetFullPath("toastImageAndText.png");
                if (toastData.Image != null)
                {
                    toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText02);
                    XmlNodeList imageElements = toastXml.GetElementsByTagName("image");
                    imageElements[0].Attributes.GetNamedItem("src").NodeValue = "file:///" + toastData.Image;
                    //imageElements[0].Attributes.GetNamedItem("src").NodeValue = toastData.Image;
                    if (toastData.Header != null)
                    {
                        // Fill in the text elements
                        XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
                        stringElements[0].AppendChild(toastXml.CreateTextNode(toastData.Header));
                        stringElements[1].AppendChild(toastXml.CreateTextNode(toastData.Message));
                    }
                    else
                    {
                        // Fill in the text elements
                        XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
                        stringElements[0].AppendChild(toastXml.CreateTextNode(toastData.Message));
                    }
                }
                else if (toastData.Header != null)
                {
                    toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
                    // Fill in the text elements
                    XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
                    stringElements[0].AppendChild(toastXml.CreateTextNode(toastData.Header));
                    stringElements[1].AppendChild(toastXml.CreateTextNode(toastData.Message));
                }
                else
                {
                    toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
                    // Fill in the text elements
                    XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
                    stringElements[0].AppendChild(toastXml.CreateTextNode(toastData.Message));
                }

                // Set if silent or not
                SetSilent(toastData.Sound, toastXml);
            }

            else
            {
                toastXml = new XmlDocument();
                toastXml.LoadXml(toastData.Xml);
                // Create the toast and attach event listeners
            }


            //string xml = toastXml.GetXml();

            // Create the toast and attach event listeners
            ToastNotification toast = new ToastNotification(toastXml);
            if (toastData.Command != null)
            { toast.Activated += CommandBuilder.BuildCommand(toastData.Command); }
            else
            {
                toast.Activated += ToastActivated;
            }
            toast.Dismissed += ToastDismissedMock;
            toast.Failed += ToastFailed;

            // Show the toast. Be sure to specify the AppUserModelId on your application's shortcut!
            ToastNotificationManager.CreateToastNotifier(APP_ID).Show(toast);
            return toast;
        }

        //Toasts.ToastFromXml(File.ReadAllText(toastData.Xml));

        static ToastNotification ShowFromXml(ToastModel toastData)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(toastData.Xml);
            // Create the toast and attach event listeners
            ToastNotification toast = new ToastNotification(xmlDoc);
            if (toastData.Command != null)
            { toast.Activated += CommandBuilder.BuildCommand(toastData.Command); }
            else
            {
                toast.Activated += ToastActivated;
            }
            toast.Dismissed += ToastDismissedMock;
            toast.Failed += ToastFailed;

            return toast;
        }

        public static ToastNotification ShowToast(ToastModel toastData, string appId = null)
        {
            if (toastData.Xml == null)
            {
                ToastXmlBuilder toastBuilder = new ToastXmlBuilder();
                if (toastData.Header != null)
                {
                    toastBuilder.AddText(toastData.Header);
                }
                if (toastData.Title != null)
                {
                    toastBuilder.AddText(toastData.Title);
                }
                if (toastData.Message != null)
                {
                    toastBuilder.AddText(toastData.Message);
                }
                if (toastData.Text != null)
                {
                    toastBuilder.AddText(toastData.Text);
                }
                if (toastData.Logo != null)
                {
                    toastBuilder.AddLogo(toastData.Logo);
                }
                if (toastData.Image != null)
                {
                    toastBuilder.AddImage(toastData.Image);
                }
                if (toastData.HeroImage != null)
                {
                    toastBuilder.AddHeroImage(toastData.Image);
                }
                if (toastData.Input != null)
                {
                    toastBuilder.AddInput(toastData.Input.Id, toastData.Input.Type, toastData.Input.Content, toastData.Input.DefaultInput, toastData.Input.Values);
                }
                if (toastData.Inputs != null)
                {
                    foreach (var input in toastData.Inputs)
                    {
                        toastBuilder.AddInput(input.Id, input.Type, input.Content, input.DefaultInput, input.Values);
                    }
                    //string arg = $"type={}&amp;data={}";
                    //toastBuilder.AddButton(toastData.Button.Caption, toastData.Button.Command.Data, toastData.Button.Image);
                }
                if (toastData.Button != null)
                {
                    //string arg = $"type={}&amp;data={}";
                    string command = CommandToArgs(toastData.Button.Command);
                    toastBuilder.AddButton(toastData.Button.Caption, command, toastData.Button.Image, toastData.Button.LinkedId);
                }
                if (toastData.Buttons != null)
                {
                    foreach (var button in toastData.Buttons)
                    {
                        string command = CommandToArgs(button.Command);
                        toastBuilder.AddButton(button.Caption, command, button.Image, button.LinkedId);
                    }
                    //string arg = $"type={}&amp;data={}";
                    //toastBuilder.AddButton(toastData.Button.Caption, toastData.Button.Command.Data, toastData.Button.Image);
                }
                if (toastData.OnClick != null)
                {
                    string command = CommandToArgs(toastData.OnClick);
                    toastBuilder.SetOnClickCommand(command);
                }
                toastData.Xml = toastBuilder.ToXml();
            }
            Console.WriteLine("XML:\n");
            Console.WriteLine(toastData.Xml);

            //string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<toast launch=\"type=openLink&amp;link=www.google.com\" activationType=\"foreground\">\n<visual>\n<binding template=\"ToastGeneric\">\n<image src=\"D:\\img\\картинки тест\\rad.jpg\" placement=\"appLogoOverride\" hint-crop=\"circle\" id=\"0\"/>\n<text id=\"0\">title</text>\n<text id=\"1\">message</text>\n</binding>\n</visual>\n<audio silent=\"False\"/>\n<actions>\n<input id=\"text\" type=\"text\" />\n<action content=\"Send answer\" arguments=\"type=openLink&amp;link=storageup://chat/555&amp;sourceId[]=text\" />\n</actions>\n</toast>\n";
            //toastData.Xml = xml;

            //Console.WriteLine("XML:\n");
            //Console.WriteLine(toastData.Xml);
            XmlDocument toastXml = new XmlDocument();
            toastXml.LoadXml(toastData.Xml);

            ToastNotification toast = new ToastNotification(toastXml);
            if (toastData.OnClick != null)
            {
                //toast.Activated += CommandBuilder.BuildCommand(toastData.OnClick);
                //toast.Dismissed += (o, e) => { (CommandBuilder.BuildCommand(toastData.OnClick)).Invoke(o,e); };
                toast.Activated += ToastActivated;
            }
            else
            {
                toast.Activated += ToastActivated;
            }
            //toast.Dismissed += ToastDismissedMock;
            toast.Failed += ToastFailed;
            toast.Dismissed += ToastDismissedMock;
            String app_id = appId ?? toastData.Source ?? Process.GetCurrentProcess().MainModule.FileName;
            // Show the toast. Be sure to specify the AppUserModelId on your application's shortcut!
            ToastNotificationManager.CreateToastNotifier(app_id).Show(toast);
            return toast;
        }


        //public static ToastNotification ShowToast(ToastModel toastData, string appId = null)
        //{
        //    if (toastData.Xml == null)
        //    {
        //        ToastXmlBuilder toastBuilder = new ToastXmlBuilder();
        //        toastBuilder.Open();
        //        toastBuilder.Visual = new VisualXmlBuilder();
        //        toastBuilder.Visual.Open();
        //        if (toastData.Header != null)
        //        {
        //            toastBuilder.Visual.AddText(toastData.Header);
        //        }
        //        if (toastData.Title != null)
        //        {
        //            toastBuilder.Visual.AddText(toastData.Title);
        //        }
        //        if (toastData.Message != null)
        //        {
        //            toastBuilder.Visual.AddText(toastData.Message);
        //        }
        //        if (toastData.Text != null)
        //        {
        //            toastBuilder.Visual.AddText(toastData.Text);
        //        }
        //        if (toastData.Logo != null)
        //        {
        //            toastBuilder.Visual.AddLogo(toastData.Logo);
        //        }
        //        if (toastData.Image != null)
        //        {
        //            toastBuilder.Visual.AddImage(toastData.Image);
        //        }
        //        if (toastData.HeroImage != null)
        //        {
        //            toastBuilder.Visual.AddHeroImage(toastData.Image);
        //        }
        //        toastBuilder.Visual.Close();
        //        toastBuilder.Close();
        //        toastData.Xml = toastBuilder.ToXml();
        //    }

        //    XmlDocument toastXml = new XmlDocument();
        //    toastXml.LoadXml(toastData.Xml);

        //    ToastNotification toast = new ToastNotification(toastXml);
        //    if (toastData.OnClick != null)
        //    {
        //        toast.Activated += CommandBuilder.BuildCommand(toastData.OnClick);
        //        toast.Dismissed += (o, e) => { (CommandBuilder.BuildCommand(toastData.OnClick)).Invoke(o, e); };
        //    }
        //    else
        //    {
        //        toast.Activated += ToastActivatedMock;
        //    }
        //    toast.Dismissed += ToastDismissedMock;
        //    toast.Failed += ToastFailed;
        //    String app_id = appId ?? toastData.Source ?? Process.GetCurrentProcess().MainModule.FileName;
        //    Show the toast.Be sure to specify the AppUserModelId on your application's shortcut!
        //    ToastNotificationManager.CreateToastNotifier(app_id).Show(toast);
        //    return toast;
        //}

        public static void SetSilent(bool useSound, XmlDocument toastXml)
        {
            var audio = toastXml.GetElementsByTagName("audio").FirstOrDefault();

            if (audio == null)
            {
                audio = toastXml.CreateElement("audio");
                var toastNode = ((XmlElement)toastXml.SelectSingleNode("/toast"));

                if (toastNode != null)
                {
                    toastNode.AppendChild(audio);
                }
            }
            var attribute = toastXml.CreateAttribute("silent");
            attribute.Value = (!useSound).ToString().ToLower();
            audio.Attributes.SetNamedItem(attribute);
        }

        public static void ToastFromData(ToastModel toastData)
        {
            // Get a toast XML template
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText04);
            string xml = toastXml.GetXml();
            //Console.WriteLine(xml);

            if (toastData.Header != null || toastData.Message != null)
            {
                // Fill in the text elements
                XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
                for (int i = 0; i < stringElements.Length; i++)
                {
                    var element = stringElements[i];
                    //stringElements[i].AppendChild(toastXml.CreateTextNode($"{(i == 0 ? "Title":$"Line {i}")} "));

                    if (i == 0 && toastData.Header != null)
                    {
                        element.AppendChild(toastXml.CreateTextNode(toastData.Header));
                    }
                    else
                        if (toastData.Message != null)
                    {
                        element.AppendChild(toastXml.CreateTextNode(toastData.Message));
                        break;
                    }
                }
            }

            if (toastData.Image != null)
            {
                // Specify the absolute path to an image as a URI
                //String imagePath = new System.Uri(Path.GetFullPath("StorageUp.ico")).AbsoluteUri;
                String imagePath = new System.Uri(Path.GetFullPath(toastData.Image)).AbsoluteUri;
                XmlNodeList imageElements = toastXml.GetElementsByTagName("image");
                imageElements[0].Attributes.GetNamedItem("src").NodeValue = imagePath;
                var attr = imageElements[0].Attributes.ToList();
            }

            // Create the toast and attach event listeners
            ToastNotification toast = new ToastNotification(toastXml);
            //toast.Activated += ToastActivated;
            if (toastData.Command != null)
            { toast.Activated += CommandBuilder.BuildCommand(toastData.Command); }
            else
            {
                toast.Activated += ToastActivated;
            }
            toast.Dismissed += ToastDismissedMock;
            toast.Failed += ToastFailed;

            // Show the toast. Be sure to specify the AppUserModelId on your application's shortcut!
            ToastNotificationManager.CreateToastNotifier(APP_ID).Show(toast);
        }

        public static void ToastFromXml(String xml)
        {
            /*
            
            xml = @"<toast>  
                              <visual>
                                <binding template=""ToastGeneric"">
                                  <text>Hello sup</text>
                                  <text>This is a simple toast message</text>
                                </binding>
                              </visual>  
                            </toast>";

            */

            /*

            xml = @"<toast launch=""action=viewPhoto&amp;photoId=92187"">

                  <visual>
                    <binding template=""ToastGeneric"">
                      <image placement=""appLogoOverride"" hint-crop=""circle"" src=""https://unsplash.it/64?image=669""/>
                      <text>Adam Wilson tagged you in a photo</text>
                      <text>On top of McClellan Butte - with Andrew Bares</text>
                      <image src=""https://unsplash.it/360/202?image=883""/>
                    </binding>
                  </visual>

                  <actions>

                    <action
                      content=""Like""
                      activationType=""background""
                      arguments=""likePhoto&amp;photoId=92187""/>
    
                    <action
                      content=""Comment""
                      arguments=""action=commentPhoto&amp;photoId=92187""/>
    
                  </actions>

                </toast>";
            
            */

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            ToastNotification toast = new ToastNotification(doc);
            ToastNotificationManager.CreateToastNotifier(APP_ID).Show(toast);

        }

        static void Toast()
        {
            //new AppNotificationBuilder();
            //var toastContent = new ToastContent()
            //{
            //    Visual = new ToastVisual()
            //    {
            //        BindingGeneric = new ToastBindingGeneric()
            //        {
            //            Children =
            //        {
            //            new AdaptiveText()
            //            {
            //                Text = "StorageUp"
            //            },
            //            new AdaptiveText()
            //            {
            //                Text = "New version avaiable"
            //            }
            //        }
            //        }
            //    },
            //    Actions = new ToastActionsCustom()
            //    {
            //        Buttons =
            //    {
            //        new ToastButtonSnooze("Install")
            //        {
            //            SelectionBoxId = "snoozeTime"
            //        },
            //        new ToastButtonDismiss("Dismiss")
            //    }
            //    }
            //};
        }


        private static void ToastActivated(ToastNotification sender, object e)
        {
            Console.WriteLine("Toast Activated");
            var toasterArgs = e as ToastActivatedEventArgs;
            if (toasterArgs == null) { Console.WriteLine("toast arguments are null"); return; }

            string arguments = toasterArgs.Arguments;
            var inputs = toasterArgs.UserInput;         

            var type = HttpUtility.ParseQueryString(arguments).Get("type");
            if (type == null)
            {
                Console.WriteLine("toast type is null");
                return;
            }

            switch (type)
            {
                case "openLink":
                    CommandBuilder.ParseOpenLinkCommand(arguments, inputs);
                    break;
                case "runApp":
                    CommandBuilder.ParseRunAppCommand(arguments, inputs);
                    break;
                case "package":
                    CommandBuilder.ParseSendPackageCommand(arguments, inputs);
                    break;
                case "rest":
                    CommandBuilder.ParseRestCommand(arguments, inputs);                    
                    break;
            }            

            //Environment.Exit(0);
        }

        private static void ToastDismissedMock(ToastNotification sender, ToastDismissedEventArgs e)
        {
            String outputText = "";
            int exitCode = -1;
            switch (e.Reason)
            {
                case ToastDismissalReason.ApplicationHidden:
                    outputText = "Toast Hidden";
                    exitCode = 1;
                    break;
                case ToastDismissalReason.UserCanceled:
                    outputText = "Toast Dismissed";
                    exitCode = 2;
                    break;
                case ToastDismissalReason.TimedOut:
                    outputText = "Toast Timeout";
                    exitCode = 3;
                    break;
            }
            Console.WriteLine(outputText);
            //Environment.Exit(exitCode);
        }

        private static void ToastFailed(ToastNotification sender, ToastFailedEventArgs e)
        {
            Console.WriteLine("Toast Error");
            //Environment.Exit(-1);
        }

        static string CommandToArgs(CommandModel command)
        {
            //List<string> result = new List<string>();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"type={command.Type}");
            switch (command.Type) {
                case "openLink":
                    stringBuilder.Append($"{Separator}link={command.Link}");
                    if (command.Data != null) { stringBuilder.Append($"{Separator}data={command.Data}"); }
                    if (command.SourceId != null && command.SourceId.Length > 0)
                    {
                        foreach (var source in command.SourceId)
                        {
                            stringBuilder.Append($"{Separator}sourceId[]={source}");
                        }
                    }
                    break;
                case "runApp":
                    stringBuilder.Append($"{Separator}appPath={command.AppPath}");
                    if (command.Data != null) { stringBuilder.Append($"{Separator}data={command.Data}"); }
                    if (command.SourceId != null && command.SourceId.Length > 0)
                    {
                        foreach (var source in command.SourceId)
                        {
                            stringBuilder.Append($"{Separator}sourceId[]={source}");
                        }
                    }
                    break;
                case "package":
                    stringBuilder.Append($"{Separator}protocol={command.Protocol}");
                    stringBuilder.Append($"{Separator}ip={command.Ip}");
                    stringBuilder.Append($"{Separator}port={command.Port}");
                    if (command.Data != null) { stringBuilder.Append($"{Separator}data={command.Data}"); }
                    if (command.SourceId !=null &&  command.SourceId.Length > 0)
                    {
                        foreach(var source in command.SourceId)
                        {
                            stringBuilder.Append($"{Separator}sourceId[]={source}");
                        }
                    }
                    break;
                case "rest":
                    stringBuilder.Append($"{Separator}baseUrl={command.BaseUrl}");
                    stringBuilder.Append($"{Separator}request={command.Request}");
                    if (command.Data != null) { stringBuilder.Append($"{Separator}data={command.Data}"); }
                    if (command.SourceId != null && command.SourceId.Length > 0)
                    {
                        foreach (var source in command.SourceId)
                        {
                            stringBuilder.Append($"{Separator}sourceId[]={source}");
                        }
                    }
                    break;
            }
            return stringBuilder.ToString();
        }
    }
}