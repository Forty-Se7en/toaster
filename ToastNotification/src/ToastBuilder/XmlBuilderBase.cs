using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification
{
    internal abstract class XmlBuilderBase
    {
        protected string _xml = "";

        public bool IsClosed { get; protected set; } = false;

        public string ToXml()
        {
            return _xml;
        }

        public override string ToString()
        {
            return ToXml();
        }

        public abstract void Open(); 

        public abstract void Close();

        protected void CheckState()
        {
            if (IsClosed) throw new ObjectDisposedException(nameof(this.GetType) + " was already closed");
        }
    }
}
