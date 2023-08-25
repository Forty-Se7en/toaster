using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace ToastHelpers
{
    public class Registrator
    {
        //public const String APP_ID = "{D65231B0-B2F1-4857-A4CE-A8E7C6EA7D27}\\WindowsPowerShell\\v1.0\\powershell.exe";
        //private const String APP_ID = "{D65231B0-B2F1-4857-A4CE-A8E7C6EA7D27}\\cmd.exe";
        //private const String APP_ID = "C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe";
        //private static String APP_ID = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.SystemX86), "\\WindowsPowerShell\\v1.0\\powershell.exe");
        private static String APP_ID = "toaster";

        public static void RegisterApp(String exePath, string AppId = null)
        {
            string shortcutDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                + "\\Microsoft\\Windows\\Start Menu\\Programs";
            if (!Directory.Exists(shortcutDir))
            {
                try
                {
                    Directory.CreateDirectory(shortcutDir);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            string appName = exePath.Split('\\').Last().Split('.').First();
            string shortcutPath = shortcutDir + "\\" + appName + ".lnk";

            if (AppId == null) AppId = exePath;

            try
            {
                if (!File.Exists(shortcutPath))
                {
                    InstallShortcut(shortcutPath, exePath, AppId);
                    RegisterComServer(exePath);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        public static void RegisterSelf(string appId = null)
        {
            // Find the path to the current executable
            String exePath = Process.GetCurrentProcess().MainModule.FileName;
            RegisterApp(exePath, appId);
        }



        public static void RegisterAppForNotificationSupport()
        {
            string shortcutDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                + "\\Microsoft\\Windows\\Start Menu\\Programs";
            if (!Directory.Exists(shortcutDir))
            {
                try
                {
                    Directory.CreateDirectory(shortcutDir);
                } catch (Exception e) {
                    Console.WriteLine(e);
                }
            }

            String exePath = Process.GetCurrentProcess().MainModule.FileName;
            string appName = exePath.Split('\\').Last().Split('.').First();
            string shortcutPath = shortcutDir + "\\" + appName + ".lnk";

            //string shortcutPath = shortcutDir + "\\toaster.lnk";
            try
            {
                if (!File.Exists(shortcutPath))
                {
                    
                    // Find the path to the current executable
                    //String exePath = Process.GetCurrentProcess().MainModule.FileName;

                    InstallShortcut(shortcutPath, exePath, APP_ID);
                    RegisterComServer(exePath);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        static void InstallShortcut(String shortcutPath, String exePath, String appId)
        {
            try
            {
                IShellLinkW newShortcut = (IShellLinkW)new CShellLink();

                // Create a shortcut to the exe
                newShortcut.SetPath(exePath);

                // Open the shortcut property store, set the AppUserModelId property
                IPropertyStore newShortcutProperties = (IPropertyStore)newShortcut;

                PropVariantHelper varAppId = new PropVariantHelper();
                varAppId.SetValue(appId);
                newShortcutProperties.SetValue(PROPERTYKEY.AppUserModel_ID, varAppId.Propvariant);

                PropVariantHelper varToastId = new PropVariantHelper();
                varToastId.VarType = VarEnum.VT_CLSID;
                varToastId.SetValue(typeof(NotificationActivator).GUID);

                newShortcutProperties.SetValue(PROPERTYKEY.AppUserModel_ToastActivatorCLSID, varToastId.Propvariant);

                // Commit the shortcut to disk
                IPersistFile newShortcutSave = (IPersistFile)newShortcut;

                newShortcutSave.Save(shortcutPath, true);

            } catch (Exception e) {
                Console.WriteLine(e);
            }
        }

        static void RegisterComServer(String exePath)
        {
            try {
                // We register the app process itself to start up when the notification is activated, but
                // other options like launching a background process instead that then decides to launch
                // the UI as needed.
                string regString = String.Format("SOFTWARE\\Classes\\CLSID\\{{{0}}}\\LocalServer32", typeof(NotificationActivator).GUID);
                var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regString);
                key.SetValue(null, exePath);

                using (RegistryKey rk = Registry.CurrentUser.CreateSubKey(regString))
                {
                    if (rk != null)
                    {
                        rk.SetValue(null, exePath);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
