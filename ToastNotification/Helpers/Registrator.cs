using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Helpers
{
    internal class Registrator
    {
        public const String APP_ID = "StorageUpToast";

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
            string shortcutPath = shortcutDir + "\\StorageUpToast.lnk";
            try
            {
                if (!File.Exists(shortcutPath))
                {
                    // Find the path to the current executable
                    String exePath = Process.GetCurrentProcess().MainModule.FileName;
                    InstallShortcut(shortcutPath, exePath);
                    RegisterComServer(exePath);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        static void InstallShortcut(String shortcutPath, String exePath)
        {
            try
            {
                IShellLinkW newShortcut = (IShellLinkW)new CShellLink();

                // Create a shortcut to the exe
                newShortcut.SetPath(exePath);

                // Open the shortcut property store, set the AppUserModelId property
                IPropertyStore newShortcutProperties = (IPropertyStore)newShortcut;

                PropVariantHelper varAppId = new PropVariantHelper();
                varAppId.SetValue(APP_ID);
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
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
