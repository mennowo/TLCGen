using System.Collections.Generic;
using TLCGen.Models;

namespace TLCGen.Messaging.Messages
{
	public class SelectieveDetectorenChangedMessage
    {
        public List<SelectieveDetectorModel> AddedDetectoren { get; }
        public List<SelectieveDetectorModel> RemovedDetectoren { get; }

        public SelectieveDetectorenChangedMessage(List<SelectieveDetectorModel> added, List<SelectieveDetectorModel> removed)
        {
            AddedDetectoren = added;
            RemovedDetectoren = removed;
        }
    }
}
