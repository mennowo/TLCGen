using GalaSoft.MvvmLight.Messaging;
using TLCGen.Models.Enumerations;

namespace TLCGen.Messaging.Messages
{
	public class FaseDetectorTypeChangedMessage : MessageBase
    {
        public DetectorTypeEnum Type { get; }
        public string DetectorDefine { get; }

        public FaseDetectorTypeChangedMessage(string detectordefine, DetectorTypeEnum type)
        {
            Type = type;
            DetectorDefine = detectordefine;
        }
    }
}
