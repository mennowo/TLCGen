using System.Collections.Generic;
using GalaSoft.MvvmLight.Messaging;
using TLCGen.Models;

namespace TLCGen.Messaging.Messages
{
	public class DetectorenExtraListChangedMessage : MessageBase
    {
        public List<DetectorModel> DetectorenList { get; private set; }

        public DetectorenExtraListChangedMessage(List<DetectorModel> detectorenlist)
        {
            DetectorenList = detectorenlist;
        }
    }
}
