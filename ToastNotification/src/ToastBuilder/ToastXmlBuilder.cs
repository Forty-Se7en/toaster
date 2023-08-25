using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification
{
    internal class ToastXmlBuilder : XmlBuilderBase
    { 
        private const string _openToast = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<toast activationType=\"foreground\">\r\n";
        private const string _closeToast = "</toast>";

        public const string _silent = "<audio silent=\"true\"/>";

        public VisualXmlBuilder Visual;
        public ActionXmlBuilder Action;

        public override void Open()
        {
            CheckState();
            _xml += _openToast;
        }

        public override void Close()
        {
            CheckState();
            if (Visual != null) { _xml += Visual.ToXml(); }
            if (Action != null) { _xml += Action.ToXml(); }
            _xml += _closeToast;
            IsClosed = true;
        }

        public void SetSilent()
        {
            CheckState();
            _xml += _silent;
        }
    }
}
