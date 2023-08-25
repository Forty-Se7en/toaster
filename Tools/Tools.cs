using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Notification.Models;

namespace Notification
{
    public class Tools
    {
        #region Console

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public const int SW_HIDE = 0;
        public const int SW_SHOW = 5;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();

        public static void ShowConsoleWindow()
        {
            var handle = GetConsoleWindow();

            if (handle == IntPtr.Zero)
            {
                AllocConsole();
            }
            else
            {
                ShowWindow(handle, SW_SHOW);
            }
        }

        public static void HideConsoleWindow()
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);
        }

        #endregion

        public static ToastModel ParseArgs(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No args provided.\n");
                //Environment.Exit(-1);
                //PrintInstructions();
            }

            var toastData = new ToastModel();

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-h":
                        if (i + 1 < args.Length)
                        {
                            toastData.Header = args[i + 1];
                        }
                        else
                        {
                            Console.WriteLine("Missing argument to -h.\n Supply argument as -h \"header string\"\n");
                            Environment.Exit(-1);
                        }
                        break;
                    case "-m":
                        if (i + 1 < args.Length)
                        {
                            toastData.Message = args[i + 1];
                        }
                        else
                        {
                            Console.WriteLine("Missing argument to -m.\n Supply argument as -m \"message string\"\n");
                            Environment.Exit(-1);
                        }
                        break;
                    case "-l":
                        if (i + 1 < args.Length)
                        {
                            toastData.Logo = args[i + 1];
                        }
                        else
                        {
                            Console.WriteLine("Missing argument to -l.\n Supply argument as -l \"logo path\"\n");
                            Environment.Exit(-1);
                        }
                        break;

                    case "-i":
                        if (i + 1 < args.Length)
                        {
                            toastData.Image = args[i + 1];
                        }
                        else
                        {
                            Console.WriteLine("Missing argument to -i.\n Supply argument as -i \"image path\"\n");
                            Environment.Exit(-1);
                        }
                        break;
                    case "-b":
                        if (i + 2 < args.Length)
                        {
                            //toastData.ButtonText = args[i + 1];
                            //toastData.ButtonCommand = args[i + 2];
                        }
                        else
                        {
                            Console.WriteLine("Missing arguments to -b.\n Supply argument as -b \"button text\" \"button command\"\n");
                            Environment.Exit(-1);
                        }
                        break;
                    case "-s":
                        {
                            if (i + 1 < args.Length)
                            {
                                try
                                {
                                    toastData.Sound = Convert.ToBoolean(args[i + 1]);
                                }
                                catch 
                                {
                                    Console.WriteLine("Failed to read -s (sound) value");
                                    Environment.Exit(-1);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Missing argument to -s.\n Supply argument as -s \"toast sound\"\n");
                                Environment.Exit(-1);
                            }
                            break;

                        }
                    case "-xml":
                        {
                            if (i + 1 < args.Length)
                            {
                                toastData.Xml = args[i + 1];
                            }
                            else
                            {
                                Console.WriteLine("Missing argument to -xml.\n Supply argument as -xml \"path to xml\"\n");
                                Environment.Exit(-1);
                            }

                            break;
                        }
                    //case "-w":
                    //    _wait = true;
                    //    break;

                    default: break;
                }
            }
            return toastData;
        }

        public static string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
