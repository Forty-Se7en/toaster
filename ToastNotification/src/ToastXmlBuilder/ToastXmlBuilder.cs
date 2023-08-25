using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;

namespace Notification
{
    class ToastXmlBuilder
    {
        bool _silent = false;
        ImageObject _logo;
        ImageObject _heroImage;

        int _textIdIndex = 0;
        int _imageIdIndex = 2;
        int _buttonIdIndex = 0;

        readonly List<XmlObject> _visual = new List<XmlObject>();

        readonly List<XmlObject> _actions = new List<XmlObject>();

        public void AddHeader(String text)
        {
            _visual.Insert(0, new TextObject(text, _textIdIndex++));
        }

        public void AddText(String text)
        {
            _visual.Add(new TextObject(text, _textIdIndex++));
        }

        public void AddLogo(String logo)
        {
            _logo = new ImageObject(logo, 0, type: ImageType.logo);
        }

        public void AddHeroImage(String image)
        {
            _heroImage = new ImageObject(image, 1, type: ImageType.hero);
        }

        public void AddImage(String image)
        {
            _visual.Add(new ImageObject(image, _imageIdIndex++));
        }

        //void addButton(String caption, String action) {}

        public void SetSilent(bool silent)
        {
            _silent = silent;
        }

        public String ToXml()
        {
            String xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n";
            xml += "<toast activationType=\"background\">\n";

            // visual
            xml += "<visual>\n";
            xml += "<binding template=\"ToastGeneric\">\n";
            if (_logo != null)
            {
                xml += _logo.ToXml();
            }
            if (_heroImage != null)
            {
                xml += _heroImage.ToXml();
            }
            foreach (var visual in _visual)
            {
                xml += visual.ToXml();
            }
            xml += "</binding>\n";
            xml += "</visual>\n";

            // audio
            xml += $"<audio silent=\"{_silent}\"/>\n";

            // actions

            xml += "</toast>\n";
            return xml;
        }
    }

    abstract class XmlObject
    {
        protected readonly int _id;

        protected XmlObject(int id) { this._id = id; }

        public abstract String ToXml();
        
        public override String ToString()
        {
            return ToXml();
        }
    }

    abstract class VisualObject: XmlObject
    {
        protected String _data;

        protected VisualObject(String data, int id) : base(id)
        {
            _data = data;
        }
    }

    class TextObject : VisualObject
    {
        public TextObject(string data, int id) : base(data, id) { }

        public override String ToXml() {
            return $"<text id=\"{_id}\">{_data}</text>\n";
        }
    }

    class ImageObject: VisualObject
    {
        ImageType _type;
        public ImageObject(string data, int id, ImageType type = ImageType.none) : base(data, id) 
        { 
            this._type = type;
        }
        
      public override String ToXml() {
            switch (_type)
            {
                case ImageType.logo:
                    return $"<image src=\"{_data}\" placement=\"appLogoOverride\" hint-crop=\"circle\" id=\"{_id}\"/>\n";
                case ImageType.hero:
                    return $"<image src=\"{_data}\" placement=\"hero\" id=\"{_id}\"/>\n";
                default:
                    return $"<image src=\"{_data}\" id=\"{_id}\"/>\n";
            }
        }
    }

    enum ImageType { none, logo, hero }

}
