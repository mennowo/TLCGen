using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.Messaging.Messages
{
    public class DetectorenExtraListChangedMessage
    {
        public List<DetectorModel> DetectorenList { get; private set; }

        public DetectorenExtraListChangedMessage(List<DetectorModel> detectorenlist)
        {
            DetectorenList = detectorenlist;
        }
    }
}
