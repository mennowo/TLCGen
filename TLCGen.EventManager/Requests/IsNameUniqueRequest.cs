using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Messaging.Requests
{
    public class IsNameUniqueRequest
    {
        public bool Handled { get; set; }
        public bool IsUnique { get; set; }
        public string Name { get; set; }

        public IsNameUniqueRequest(string name)
        {
            Handled = false;
            IsUnique = false;
            Name = name;
        }
    }
}
