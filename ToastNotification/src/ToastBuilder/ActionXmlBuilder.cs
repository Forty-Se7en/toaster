using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification
{
    internal class ActionXmlBuilder : XmlBuilderBase
    {
        private const string _openActions = "<actions>";
        private const string _closeActions = "</actions>\r\n";

        public override void Open()
        {
            CheckState();
            _xml += _openActions;
        }

        public override void Close()
        {
            CheckState();
            _xml += _closeActions;
            IsClosed = true;
        }      
        
        public void AddButton(string caption)
        {

        }
    }
}
