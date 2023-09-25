using System;

namespace Notification.Models
{
    public class ButtonModel : ICloneable
    {
        public string Caption { get; set; }

        public string Image { get; set; }

        public string LinkedId { get; set; }

        public CommandModel Command { get; set; }

        //public ButtonModel Clone()
        //{
        //    return new ButtonModel { Caption = Caption, Image = Image, Command = Command.Clone() };
        //}

        public object Clone()
        {
           var button = (ButtonModel)this.MemberwiseClone();
            button.Command = (CommandModel)Command.Clone();
            return button;
        }
    }
}
