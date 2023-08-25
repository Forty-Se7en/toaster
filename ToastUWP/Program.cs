using Listener;
using Microsoft.Toolkit.Uwp.Notifications;
using Notification;
using Notification.Models;
using System.Text;
using System.Windows.Forms;
using ToastUWP.Toasts;
using Windows.Foundation.Collections;

internal class Program
{
    private const int START_PORT = 11000;
    private const int END_PORT = 11050;

    static ToastModel lastToast;

    private static void Main(string[] args)
    {
        //ToastGen.Generate(new ToastModel { Image = "https://static.vecteezy.com/system/resources/previews/003/731/316/original/web-icon-line-on-white-background-image-for-web-presentation-logo-icon-symbol-free-vector.jpg" }).Show();

        ToastNotificationManagerCompat.OnActivated += toastArgs =>
        {
            //Get the activation args, if you need those.
            ToastArguments args = ToastArguments.Parse(toastArgs.Argument);
            //Get user input if there's any and if you need those.
            ValueSet userInput = toastArgs.UserInput;
            //if the app instance just started after clicking on a notification 
            if (ToastNotificationManagerCompat.WasCurrentProcessToastActivated())
            {
                Console.WriteLine("App was not running, " +
                    "but started and activated by click on a notification.");
                Application.Exit();
            }
            else
            {
                Console.WriteLine("App was running, " +
                    "and activated by click on a notification.");
                if (lastToast != null && lastToast.OnClick != null)
                {
                    ToastGen.ExecuteCommand(lastToast.OnClick);
                }
            }
        };
        if (ToastNotificationManagerCompat.WasCurrentProcessToastActivated())
        {
            Console.WriteLine("ToastNotificationManagerCompat.WasCurrentProcessToastActivated()");
            //Do not show any window
            //Application.Run();
        }
        else
        {
            Console.WriteLine("ToastNotificationManagerCompat.WasCurrentProcessToastActivated()");
            //Show the main form
            //Application.Run(new Form1());
        }
        //ToastModel toastData = Mock();

        //TryCreateShortcut();
        //new UpdateToast(toastData).Show();
        //Console.ReadLine();

        if (args.Length != 0)
        {
            ToastModel toastData = Tools.ParseArgs(args);
            ShowToast(toastData);
            Thread.Sleep(1);
        }
        else
        {
            StartListen();
        }

    }

    static void StartListen()
    {
        var listener = new UDPListener(START_PORT, END_PORT);
        listener.Received += (bytes) =>
        {
            try
            {
                Parse(bytes);
            }
            catch (Exception e) { Console.WriteLine("RECEIVED exception: " + e); }
        };
        listener.StartListen();
    }

    static void Parse(byte[] bytes)
    {
        try
        {
            var inputString = Uri.UnescapeDataString(Encoding.ASCII.GetString(bytes, 0, bytes.Length));
            Console.WriteLine($"Parse: Input string: \n{inputString}\n");
            var toastData = Tools.Deserialize<ToastModel>(inputString);            
            ShowToast(toastData);
        }
        catch (Exception e)
        {
            Console.WriteLine("PARSE exception: " + e);
        }
    }

    static void ShowToast(ToastModel toastData)
    {
        lastToast = toastData;
        try
        {
            if (toastData.Xml == null)
            {
                var toast = ToastGen.GenerateToast(toastData);
                toast.Show();

                //new UpdateToast(toastData).Show();
            }
            else
            {
                if (File.Exists(toastData.Xml))
                {
                    //Toasts.ToastFromXml(File.ReadAllText(toastData.Xml));
                }
                else
                {
                    Console.WriteLine($"ShowToast: Xml file \"{toastData.Xml}\" did not find");
                    //Environment.Exit(-1);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("SHOW TOAST exception: " + ex);
        }
    }

    static ToastModel Mock()
    {
        return new ToastModel()
        {
            Header = "Hello world",
            Message = "this is my message",
            Image = "D:\\img\\rad.jpg",
            //Xml = "D:\\img\\toast2.xml"
            //Image = "StorageUp.ico",
        };

    }
}