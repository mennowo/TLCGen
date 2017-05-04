using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.Messaging.Messages
{
    public class OVIngreepSignaalGroepParametersChangedMessage
    {
        public OVIngreepSignaalGroepParametersModel SignaalGroepParameters { get; private set; }

        public OVIngreepSignaalGroepParametersChangedMessage(OVIngreepSignaalGroepParametersModel signaalgroepparameters)
        {
            SignaalGroepParameters = signaalgroepparameters;
        }
    }
}
