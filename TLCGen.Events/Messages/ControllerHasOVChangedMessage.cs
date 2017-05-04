using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models.Enumerations;

namespace TLCGen.Messaging.Messages
{
    public class ControllerHasOVChangedMessage
    {
        public OVIngreepTypeEnum Type { get; private set; }

        public ControllerHasOVChangedMessage(OVIngreepTypeEnum type)
        {
            Type = type;
        }
    }
}
