using System;
using System.ComponentModel;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class GroentijdModel : IComparable
    {
        #region Properties
        
        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        [Browsable(false)]
        [HasDefault(false)]
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
