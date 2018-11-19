using System.Collections.Generic;

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
