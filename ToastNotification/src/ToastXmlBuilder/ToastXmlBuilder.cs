using System;
using System.Collections.Generic;


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

        public void AddHeader(string text)
        {
            _visual.Insert(0, new TextObject(text, _textIdIndex++.ToString()));
        }

        public void AddText(String text)
        {
            _visual.Add(new TextObject(text, _textIdIndex++.ToString()));
        }

        public void AddLogo(String logo)
        {
            _logo = new ImageObject(logo, 0.ToString(), type: ImageType.logo);
        }

        public void AddHeroImage(String image)
        {
            _heroImage = new ImageObject(image, 1.ToString(), type: ImageType.hero);
        }

        public void AddImage(String image)
        {
            _visual.Add(new ImageObject(image, _imageIdIndex++.ToString()));
        }

        public void AddButton(string caption, string command, string image = null, string linkedId = null) {
            _actions.Add(new ButtonObject(_buttonIdIndex++.ToString(), caption, command, image, linkedId));
        }

        public void AddInput(string id, string type, string content = null, string defaultInput = null, string[] values = null)
        {
            _actions.Add(new InputObject(id, type, content, defaultInput, values));
        }

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
            if (_actions.Count > 0)
            {
                xml += "<actions>\n";
                foreach (var action in _actions)
                {
                    xml += action.ToXml();
                }
                xml += "</actions>\n";
            }

            xml += "</toast>\n";
            return xml;
        }
    }

    

}
