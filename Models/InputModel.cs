using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class InputModel : ICloneable
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
        public string DefaultInput { get; set; }
        public string[] Values { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
