//#define DEFAULT
//#define CORP

using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Notification.Models;
using System.Threading;
using Listener;
using ToastHelpers;
using System.Net.Http;
using System.Threading.Tasks;

namespace Notification
{
    internal class Program
    {
        private const int DEFAULT_START_PORT = 11000;
        private const int DEFAULT_END_PORT = 11010;

        private const int CORP_START_PORT = 12000;
        private const int CORP_END_PORT = 12010;

        private static int StartPort = DEFAULT_START_PORT;
        private static int EndPort = DEFAULT_END_PORT;

        static async Task Main(string[] args)
        {
            StartPort = CORP_START_PORT;
            EndPort = CORP_END_PORT;
#if DEFAULT
Console.WriteLine("DEFAULT version");
StartPort=DEFAULT_START_PORT;
EndPort=DEFAULT_END_PORT;
#elif CORP
Console.WriteLine("CORP version");
StartPort=CORP_START_PORT;
EndPort=CORP_END_PORT;
#else
            Console.WriteLine("Unknown version");
#endif

            #region comment
            /*
            Tools.HideConsoleWindow();

            Registrator.RegisterAppForNotificationSupport();
            NotificationActivator.Initialize();

            ToastData toastData = Tools.ParseArgs(args);

            if (args.Length == 0)
            {
                toastData = Mock();
            }

            if (toastData.Xml == null)
            {
                Toasts.ShowToast(toastData);
            }
            else
            {
                if (File.Exists(toastData.Xml))
                {
                    Toasts.ToastFromXml(File.ReadAllText(toastData.Xml));
                }
                else
                {
                    Console.WriteLine($"Xml file \"{toastData.Xml}\" did not find");
                    Environment.Exit(-1);
                }
            }

            Thread.Sleep(1);
            */

            //Tools.HideConsoleWindow();

            /*
            var toastData = MockXml();
            if (toastData.Source != null)
            {
                Registrator.RegisterApp(toastData.Source);
                Registrator.RegisterSelf(toastData.Source);
            }
            else Register();
            */

            //toastData = MockXml();
            //FromXml(toastData);
            //FromXml();

            //Register();
            //FromXml();
            //Registrator.RegisterSelf();

            #endregion

            /*HttpClient client = new HttpClient();
            var baseUrl = "http://localhost:4000/update";
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(5);
            await client.GetAsync(baseUrl);
            */
            FromPort(args);
        }

        static void FromPort(string[] args)
        {
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

            //ReceiveData();
            //Console.ReadLine();
            Console.WriteLine("Exiting...");
            Thread.Sleep(1000);
            
        }

        public static void FromMock()
        {
            var toastData = Test.Mock();
            ShowToast(toastData);
            Thread.Sleep(1000);
        }

        public static void FromXml(ToastModel toast = null)
        {
            var toastData = toast ?? Test.MockXml();
            ShowToast(toastData);
            Thread.Sleep(1000);
        }

        static void Register(ToastModel toastData)
        {
            //Registrator.RegisterAppForNotificationSupport();
            //NotificationActivator.Initialize();

            if (toastData.Source != null)
            {
                Registrator.RegisterApp(toastData.Source);
                Registrator.RegisterSelf(toastData.Source);
            }
            else
            {
                //Registrator.RegisterAppForNotificationSupport(); 
                Registrator.RegisterSelf();
            }

            NotificationActivator.Initialize();
        }

        static void StartListen()
        {
            var listener = new UDPListener(StartPort, EndPort);
            listener.Received += (bytes) =>
                {
                    try
                    {
                        Parse(bytes);
                    }
                    catch(Exception e) { Console.WriteLine("RECEIVED exception: " + e); }
                };
            listener.StartListen();
        }

        static void ShowToast(ToastModel toastData)
        {
            try
            {                
                Register(toastData);       
                //toastData = toastData.Clone();
                //Thread.Sleep(3000);
                //Register(toastData);
                ToastBuilder.ShowToast(toastData);
                /*
                if (toastData.Xml == null)
                {
                    var toast = Toasts.ShowToast(toastData);
                }
                else
                {
                    if (File.Exists(toastData.Xml))
                    {
                        Toasts.ToastFromXml(File.ReadAllText(toastData.Xml));

                    }
                    else
                    {
                        Console.WriteLine($"ShowToast: Xml file \"{toastData.Xml}\" did not find");
                        //Environment.Exit(-1);
                    }
                }*/
            }
            catch (Exception ex)
            {
                Console.WriteLine("SHOW TOAST exception: " + ex);
            }
        }

        static void Parse(byte[] bytes)
        {
            try
            {
                var inputString = Uri.UnescapeDataString(Encoding.ASCII.GetString(bytes, 0, bytes.Length));
                Console.WriteLine($"Parse: Input string: \n{inputString}\n");
                var toastData = Tools.Deserialize<ToastModel>(inputString);
                //if (toastData.Image != null)
                //{ 
                //    toastData.Image = Uri.UnescapeDataString(toastData.Image);
                //}
                ShowToast(toastData );
            }
            catch(Exception e)
            {
                Console.WriteLine("PARSE exception: " + e);
            }
        }      

        
        /*
        static async void ReceiveData()
        {
            var udpServer = new UdpClient(11000);
            Console.WriteLine("UDP-server was started...");

            // получаем данные
            var result = await udpServer.ReceiveAsync();
            // предположим, что отправлена строка, преобразуем байты в строку
            var message = Encoding.UTF8.GetString(result.Buffer);

            Console.WriteLine($"received {result.Buffer.Length} bytes");
            Console.WriteLine($"remote address: {result.RemoteEndPoint}");
            Console.WriteLine(message);
        }
        */

    }
}
