using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models.Enumerations;

namespace TLCGen.Messaging.Messages
{
    public class FaseDetectorVeiligheidsGroenChangedMessage
    {
        public NooitAltijdAanUitEnum VeiligheidsGroen { get; private set; }
        public string DetectorDefine { get; private set; }

        public FaseDetectorVeiligheidsGroenChangedMessage(string detectordefine, NooitAltijdAanUitEnum veiligheidsgroen)
        {
            DetectorDefine = detectordefine;
            VeiligheidsGroen = veiligheidsgroen;
        }
    }
}
