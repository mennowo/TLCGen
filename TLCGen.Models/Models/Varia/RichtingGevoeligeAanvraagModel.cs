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
        public string FaseCyclus { get; set; }
        public string VanDetector { get; set; }
        public string NaarDetector { get; set; }
        public int MaxTijdsVerschil { get; set; }
    }
}
