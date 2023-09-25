using System;

namespace Notification.Models
{
    public class CommandModel: ICloneable
    {
        public string Type { get; set; }
        public string Data { get; set; }

        //public CommandModel Clone()
        //{
        //    return new CommandModel { Type = Type, Data = Data };
        //}

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

}
