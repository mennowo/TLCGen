using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    [Serializable]
    public class RichtingGevoeligVerlengModel
    {
        public string FaseCyclus { get; set; }
        public string VanDetector { get; set; }
        public string NaarDetector { get; set; }
        public int MaxTijdsVerschil { get; set; }
        public int VerlengTijd { get; set; }
        public int HiaatTijd { get; set; }
    }
}
