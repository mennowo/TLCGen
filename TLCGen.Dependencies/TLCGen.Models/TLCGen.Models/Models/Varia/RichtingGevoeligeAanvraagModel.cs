using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    [Serializable]
    [RefersToSignalGroup("FaseCyclus")]
    [RefersToDetector("VanDetector", "NaarDetector")]
    public class RichtingGevoeligeAanvraagModel
    {
        [HasDefault(false)]
        public string FaseCyclus { get; set; }
        [HasDefault(false)]
        public string VanDetector { get; set; }
        [HasDefault(false)]
        public string NaarDetector { get; set; }
        public int MaxTijdsVerschil { get; set; }
        public bool ResetAanvraag { get; set; }
        public int ResetAanvraagTijdsduur { get; set; }
    }
}
