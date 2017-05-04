using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models.Enumerations;

namespace TLCGen.Messaging.Messages
{
    public class GroentijdenTypeChangedMessage
    {
        public GroentijdenTypeEnum Type { get; }

        public GroentijdenTypeChangedMessage(GroentijdenTypeEnum type)
        {
            Type = type;
        }
    }
}
