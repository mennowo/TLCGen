
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Messaging.Messages
{
	public class FaseDetectorTypeChangedMessage
    {
        public ControllerModel Controller { get; }
        public DetectorTypeEnum NewType { get; }
        public DetectorTypeEnum OldType { get; }
        public string DetectorDefine { get; }

        public FaseDetectorTypeChangedMessage(ControllerModel controller, string detectordefine, DetectorTypeEnum oldType, DetectorTypeEnum newType)
        {
            Controller = controller;
            NewType = newType;
            OldType = oldType;
            DetectorDefine = detectordefine;
        }
    }
}
