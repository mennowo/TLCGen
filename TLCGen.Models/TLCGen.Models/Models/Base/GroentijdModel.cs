using System;
using System.ComponentModel;

namespace TLCGen.Models
{
    [Serializable]
    [RefersToSignalGroup("FaseCyclus")]
    public class GroentijdModel : IComparable
    {
        #region Properties

        [Browsable(false)]
        public string FaseCyclus { get; set; }
        public int? Waarde { get; set; }

        #endregion // Properties

        #region IComparable

        public int CompareTo(object obj)
        {
	        if (!(obj is GroentijdModel m))
                throw new InvalidCastException();

            return string.Compare(FaseCyclus, m.FaseCyclus, StringComparison.Ordinal);
        }

        #endregion // IComparable
    }
}
