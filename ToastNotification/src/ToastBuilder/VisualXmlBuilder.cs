using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification
{
    internal class VisualXmlBuilder : XmlBuilderBase
    {
        private const string _openVisual = "<visual>\r\n    <binding template=\"ToastGeneric\">";
        private const string _closeVisual = "</binding>\r\n  </visual>\r\n";

        private int _textId = 0;
        private int _imageId = 0;

        public override void Open()
        {
            CheckState();
            _xml += _openVisual;
        }

        public override void Close()
        {
            CheckState();
            _xml += _closeVisual;
            IsClosed = true;
        }        

        public void AddText(string text)
        {
            CheckState();
            _textId++;
            _xml += $"<text id=\"{_textId}\">{text}</text>";
        }

        public void AddLogo(string path)
        {
            CheckState();
            _imageId++;
            _xml += $"<image src=\"{path}\" placement=\"appLogoOverride\" hint-crop=\"circle\" id=\"{_imageId}\"/>";
        }

        public void AddHeroImage(string path)
        {
            CheckState();
            _imageId++;
            _xml += $"<image src=\"{path}\" placement=\"hero\" id=\"{_imageId}\"/>";
        }

        public void AddImage(string path)
        {
            CheckState();
            _imageId++;
            _xml += $"<image src=\"{path}\" id=\"{_imageId}\"/>";
        }
    }
}
