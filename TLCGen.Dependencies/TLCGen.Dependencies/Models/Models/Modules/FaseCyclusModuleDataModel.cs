using System;

namespace TLCGen.Models
{
    [Serializable]
    [RefersTo("FaseCyclus")]
    public class FaseCyclusModuleDataModel : IComparable
    {
        #region Properties

        [HasDefault(false)]
        public string FaseCyclus { get; set; }
        public int ModulenVooruit { get; set; }
        public bool AlternatiefToestaan { get; set; }
        public int AlternatieveRuimte { get; set; }
        public int AlternatieveGroenTijd { get; set; }

        #endregion // Properties

        #region IComparable

        public int CompareTo(object obj)
        {
	        if (!(obj is FaseCyclusModuleDataModel mlfc))
            {
                throw new InvalidCastException();
            }
	        return string.Compare(FaseCyclus, mlfc.FaseCyclus, StringComparison.Ordinal);
        }

        #endregion // IComparable
    }
}
