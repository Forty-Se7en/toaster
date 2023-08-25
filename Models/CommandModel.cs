using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Models
{
    public class CommandModel
    {
        public string Type { get; set; }
        public string Data { get; set; }

        public CommandModel Clone()
        {
            return new CommandModel { Type = Type, Data = Data };
        }
    }

}
