

using System.Collections.Generic;
using TLCGen.Models;

namespace TLCGen.Messaging.Messages
{
	public class DetectorenChangedMessage
    {
        public List<DetectorModel> AddedDetectoren { get; }
        public List<DetectorModel> RemovedDetectoren { get; }

        public DetectorenChangedMessage(List<DetectorModel> added, List<DetectorModel> removed)
        {
            AddedDetectoren = added;
            RemovedDetectoren = removed;
        }
    }
}
