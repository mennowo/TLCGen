using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Messaging.Requests
{
    public class IsFasenConflictingRequest
    {
        public bool Handled { get; set; }
        public bool IsConflicting { get; set; }
        public string Define1 { get; set; }
        public string Define2 { get; set; }

        public IsFasenConflictingRequest(string define1, string define2)
        {
            Handled = false;
            IsConflicting = false;
            Define1 = define1;
            Define2 = define2;
        }
    }
}
