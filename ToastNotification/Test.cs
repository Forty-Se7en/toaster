using Notification.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Notification
{
    internal static class Test
    {
        public static ToastModel Mock()
        {
            return new ToastModel()
            {
                Header = "this is title",
                Message = "this is my message",
                Logo = "D:\\img\\rad.jpg",
                Image = "D:\\img\\rad.jpg",
                //Xml = "D:\\img\\toast2.xml"
                //Image = "https://static.vecteezy.com/system/resources/previews/003/731/316/original/web-icon-line-on-white-background-image-for-web-presentation-logo-icon-symbol-free-vector.jpg",
            };
        }

        public static ToastModel MockXml()
        {
            return new ToastModel()
            {
                //Xml = "<toast><visual><binding template=\"ToastImageAndText02\"><image id=\"1\" src=\"file:///D:\\img\\rad.jpg\"/><text id=\"1\">Hello world</text><text id=\"2\">this is my message</text></binding></visual><audio silent=\"true\"/></toast>\r\n"
                //Xml = "<toast><visual><binding template=\"ToastImageAndText02\"><image id=\"1\" placement=\"appLogoOverride\" hint-crop=\"circle\" src=\"file:///D:\\img\\rad.jpg\"/><text id=\"1\">Hello world</text><text id=\"2\">this is my message</text></binding></visual></toast>\r\n"
                //Xml = "<toast><visual><binding template=\"ToastGeneric\"><image id=\"1\" placement=\"hero\" src=\"file:///D:\\img\\rad.jpg\"/><text id=\"1\">Hello world</text><text id=\"2\">this is my message</text></binding></visual></toast>\r\n"
                //Xml = "<toast><visual><binding template=\"ToastGeneric\"><image id=\"1\" placement=\"hero\" src=\"file:///D:\\img\\rad.jpg\"/><text id=\"1\">Hello world</text><text id=\"2\">this is my message</text></binding></visual></toast>\r\n"
                //Xml = "<toast><visual><binding template=\"ToastGeneric\"><image id=\"1\" addImageQuery=\"true\" src=\"https://static.vecteezy.com/system/resources/previews/003/731/316/original/web-icon-line-on-white-background-image-for-web-presentation-logo-icon-symbol-free-vector.jpg\"/><text id=\"1\">Hello world</text><text id=\"2\">this is my message</text></binding></visual><audio silent=\"true\"/></toast>\r\n"
                //Xml = "<toast><visual><binding template=\"ToastGeneric\"><text id=\"1\">Hello world</text><text id=\"2\">this is my message</text></binding></visual><actions><action\r\n      activationType=\"system\"\r\n      arguments=\"dismiss\"\r\n      content=\"\"/></actions></toast>\r\n"
                //Xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<toast>\r\n<visual>\r\n    <binding template=\"ToastGeneric\"><text id=\"1\">Заголовок</text><text id=\"2\">Открыть гугол</text><image src=\"D:\\img\\картинки тест\\rad.jpg\" placement=\"appLogoOverride\" hint-crop=\"circle\" id=\"1\"/><image src=\"D:\\img\\картинки тест\\rad.jpg\" placement=\"hero\" id=\"2\"/></binding>\r\n  </visual>\r\n</toast>"
                Xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
                "<toast>\r\n" +
                "<visual>\r\n    " +
                "<binding template=\"ToastGeneric\">" +
                "<text id=\"1\">Заголовок</text>" +
                "<text id=\"2\">Открыть гугол</text>" +
                "<image src=\"D:\\img\\картинки тест\\rad.jpg\" placement=\"appLogoOverride\" hint-crop=\"circle\" id=\"1\"/>" +
                "<image src=\"D:\\img\\картинки тест\\rad.jpg\" placement=\"hero\" id=\"2\"/>" +
                "<image src=\"D:\\img\\картинки тест\\rad.jpg\" id=\"3\"/>" +
                "</binding>\r\n  " +
                "</visual>\r\n" +
                "</toast>",
                Source = "C:\\Storageup\\StorageUp.exe"
            };
        }        

    }
}
