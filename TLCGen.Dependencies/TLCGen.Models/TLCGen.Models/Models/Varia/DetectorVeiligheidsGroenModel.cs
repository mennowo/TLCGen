using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class DetectorVeiligheidsGroenModel
    {
        public NooitAltijdAanUitEnum VeiligheidsGroen { get; set; }
        public int MinimaleTijdInMG { get; set; }
        public int Tijdsduur { get; set; }
    }
}
