
using TLCGen.Models.Enumerations;

namespace TLCGen.Messaging.Messages
{
    public class FaseDetectorVeiligheidsGroenChangedMessage
    {
        public NooitAltijdAanUitEnum VeiligheidsGroen { get; }
        public string DetectorDefine { get; }

        public FaseDetectorVeiligheidsGroenChangedMessage(string detectordefine, NooitAltijdAanUitEnum veiligheidsgroen)
        {
            DetectorDefine = detectordefine;
            VeiligheidsGroen = veiligheidsgroen;
        }
    }
}
