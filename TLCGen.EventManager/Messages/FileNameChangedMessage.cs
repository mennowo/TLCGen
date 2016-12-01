using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Messaging.Messages
{
    public class FileNameChangedMessage
    {
        public string OldFileName { get; private set; }
        public string NewFileName { get; private set; }

        public FileNameChangedMessage(string newname, string oldname)
        {
            OldFileName = oldname;
            NewFileName = newname;
        }
    }
}
