using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Messaging.Messages
{
    public class NameChangedMessage
    {
        public string OldName { get; set; }
        public string NewName { get; set; }

        public NameChangedMessage(string oldname, string newname)
        {
            OldName = oldname;
            NewName = newname;
        }
    }
}
