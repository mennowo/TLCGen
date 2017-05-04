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
    public class OVIngreepSignaalGroepParametersModel : IComparable
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

        #region IComparable

        public int CompareTo(object obj)
        {
            var fcovprm = obj as OVIngreepSignaalGroepParametersModel;
            if (obj == null)
            {
                throw new NotImplementedException();
            }
            else
            {
                return this.FaseCyclus.CompareTo(fcovprm.FaseCyclus);
            }
        }

        #endregion // IComparable
    }
}
