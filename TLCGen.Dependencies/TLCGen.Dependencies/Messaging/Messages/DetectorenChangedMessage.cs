using System.Collections.Generic;
using TLCGen.Models;

namespace TLCGen.Messaging.Messages
{
    public class DetectorenChangedMessage
    {
        public ControllerModel Controller { get; }
        public List<DetectorModel> AddedDetectoren { get; }
        public List<DetectorModel> RemovedDetectoren { get; }

        public DetectorenChangedMessage(ControllerModel controller, List<DetectorModel> added, List<DetectorModel> removed)
        {
            Controller = controller;
            AddedDetectoren = added;
            RemovedDetectoren = removed;
        }
    }
}
