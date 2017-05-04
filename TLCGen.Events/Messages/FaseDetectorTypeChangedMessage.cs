using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models.Enumerations;

namespace TLCGen.Messaging.Messages
{
    public class FaseDetectorTypeChangedMessage
    {
        public DetectorTypeEnum Type { get; private set; }
        public string DetectorDefine { get; private set; }

        public FaseDetectorTypeChangedMessage(string detectordefine, DetectorTypeEnum type)
        {
            Type = type;
            DetectorDefine = detectordefine;
        }
    }
}
