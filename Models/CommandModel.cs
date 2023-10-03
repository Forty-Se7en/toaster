using System;

namespace Notification.Models
{
    public class CommandModel: ICloneable
    {
        public string Type { get; set; }

        public string Link { get; set; }

        public string AppPath { get; set; }

        public string Protocol { get; set; }

        public string Ip { get; set; }

        public string Port { get; set; }

        public string BaseUrl { get; set; }

        public string Request { get; set; }

        public string[] SourceId { get; set; }

        public string Data { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }



}
