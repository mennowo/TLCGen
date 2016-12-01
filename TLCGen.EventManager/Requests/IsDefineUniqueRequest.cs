using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Messaging.Requests
{
    public class IsDefineUniqueRequest
    {
        public bool Handled { get; set; }
        public bool IsUnique { get; set; }
        public string Define { get; set; }

        public IsDefineUniqueRequest(string define)
        {
            Handled = false;
            IsUnique = false;
            Define = define;
        }
    }
}
