using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Notification.Models;

namespace ToastUWP.Toasts
{
    internal class UpdateToast 
    {
        ToastNotification _toast;
        public const String APP_ID = "StorageUp";

        public UpdateToast(ToastModel toastInfo)
        {
            var toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                    {
                        new AdaptiveText()
                        {
                            Text = toastInfo.Header
                        },
                        new AdaptiveText()
                        {
                            Text = toastInfo.Message
                        }
                    }
                    }
                },
                Actions = new ToastActionsCustom()
                {
                    Buttons =
                {
                    new ToastButtonSnooze("Install")
                    {
                        SelectionBoxId = "snoozeTime"
                    },
                    new ToastButtonDismiss("Dismiss")
                }
                }
            };

            _toast = new ToastNotification(toastContent.GetXml());
            //ToastNotificationManager.CreateToastNotifier("sup_toaster").Show(_toast);
        }

        public void Show()
        {
            var notifier = ToastNotificationManager.CreateToastNotifier(APP_ID);
            notifier.Show(_toast);
        }
    }
}
