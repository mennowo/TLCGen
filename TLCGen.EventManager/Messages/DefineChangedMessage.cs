using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Messaging.Messages
{
    public class DefineChangedMessage
    {
        public string OldDefine { get; private set; }
        public string NewDefine { get; private set; }

        public DefineChangedMessage(string olddefine, string newdefine)
        {
            OldDefine = olddefine;
            NewDefine = newdefine;
        }
    }
}
