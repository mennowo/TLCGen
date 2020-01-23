using System;
using System.ComponentModel;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    [RefersTo(TLCGenObjectTypeEnum.Fase, "FaseCyclus")]
    public class PrioIngreepSignaalGroepParametersModel : IComparable
    {
        #region Properties

        [Browsable(false)]
        [HasDefault(false)]
        public string FaseCyclus { get; set; }
        public int AantalKerenNietAfkappen { get; set; }
        public int MinimumGroentijdConflictOVRealisatie { get; set; }
        public int PercMaxGroentijdConflictOVRealisatie { get; set; }
        public int PercMaxGroentijdVoorTerugkomen { get; set; }
        public int OndergrensNaTerugkomen { get; set; }
        public int OphoogpercentageNaAfkappen { get; set; }

        #endregion // Properties

        #region IComparable

        public int CompareTo(object obj)
        {
            if (!(obj is PrioIngreepSignaalGroepParametersModel fcovprm))
            {
                throw new InvalidCastException();
            }
            return string.Compare(FaseCyclus, fcovprm.FaseCyclus, StringComparison.Ordinal);
        }

        #endregion // IComparable
    }
}
