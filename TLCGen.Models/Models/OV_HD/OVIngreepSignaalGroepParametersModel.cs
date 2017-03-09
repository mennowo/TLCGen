using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    [Serializable]
    [RefersToSignalGroup("FaseCyclus")]
    public class OVIngreepSignaalGroepParametersModel
    {
        #region Properties

        [Browsable(false)]
        public string FaseCyclus { get; set; }
        public int AantalKerenNietAfkappen { get; set; }
        public int MinimumGroentijdConflictOVRealisatie { get; set; }
        public int PercMaxGroentijdConflictOVRealisatie { get; set; }
        public int PercMaxGroentijdVoorTerugkomen { get; set; }
        public int OndergrensNaTerugkomen { get; set; }
        public int OphoogpercentageNaAfkappen { get; set; }
        public int BlokkeertijdNaOVIngreep { get; set; }

        #endregion // Properties
    }
}
