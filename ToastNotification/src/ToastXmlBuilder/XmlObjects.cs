using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;

namespace Notification
{
    enum ImageType { none, logo, hero }

    enum CommandType { openLink, runApp, feedback }


    abstract class XmlObject
    {
        protected readonly string _id;

        protected XmlObject(string id) { this._id = id; }

        public abstract string ToXml();

        public override string ToString()
        {
            return ToXml();
        }
    }

    abstract class VisualObject : XmlObject
    {
        protected string _data;

        protected VisualObject(string data, string id) : base(id)
        {
            _data = data;
        }
    }

    class TextObject : VisualObject
    {
        public TextObject(string data, string id) : base(data, id) { }

        public override String ToXml()
        {
            return $"<text id=\"{_id}\">{_data}</text>\n";
        }
    }

    class ImageObject : VisualObject
    {
        ImageType _type;
        public ImageObject(string data, string id, ImageType type = ImageType.none) : base(data, id)
        {
            this._type = type;
        }

        public override String ToXml()
        {
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

    class ButtonObject : XmlObject
    {
        CommandType _type;
        string _caption;
        string _command;
        string _image;
        string _linkedId;

        public ButtonObject(string id, string caption, string command, string image = null, string linkedId = null) : base(id)
        {
            this._caption = caption;
            //this._type = type;
            this._command = command;
            this._image = image;
            this._linkedId = linkedId;
        }

        public override string ToXml()
        {
            string xml = $"<action content=\"{_caption}\" arguments=\"{_command}\" ";
            if (_image != null)
            {
                xml += $"imageUri=\"{_image}\" ";
            }
            if (_linkedId != null)
            {
                xml += $"hint-inputId=\"{_linkedId}\" ";
            }
            xml += "/>\n";
            return xml;
        }
    }

    class InputObject : XmlObject
    {
        string _type;
        string _content;
        string _defaultInput;
        string[] _values;

        public InputObject(string id, string type, string content = null, string defaultInputId = null, string[] values = null) : base(id)
        {
            this._content = content;
            this._type = type;
            this._defaultInput = defaultInputId;
            this._values = values;
        }

        public override string ToXml()
        {
            string xml = $"<input id=\"{_id}\" type=\"{_type}\" ";
            switch (_type)
            {
                case "text":
                    if (_content != null)
                    {
                        xml += $"placeHolderContent=\"{_content}\" ";
                    }
                    xml += "/>\n";
                    break;

                case "selection":
                    if (_values != null)
                    {
                        if (_defaultInput != null)
                        {
                            xml += $"defaultInput=\"{_defaultInput}\"";
                        }
                        xml += ">\n";
                        foreach (string value in _values)
                        {
                            xml += $"<selection id=\"{value}\" content=\"{value}\"/>\n";
                        }
                        xml += "</input>\n";
                    }
                    else
                    {
                        xml += "/>\n";
                    }
                    break;
            }
            return xml;
        }
    }

}
