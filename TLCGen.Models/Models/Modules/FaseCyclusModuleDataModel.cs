using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    [Serializable]
    [RefersToSignalGroup("FaseCyclus")]
    public class FaseCyclusModuleDataModel : IComparable
    {
        #region Properties

        public string FaseCyclus { get; set; }
        public int ModulenVooruit { get; set; }
        public bool AlternatiefToestaan { get; set; }
        public int AlternatieveRuimte { get; set; }
        public int AlternatieveGroenTijd { get; set; }

        #endregion // Properties

        #region IComparable

        public int CompareTo(object obj)
        {
            var mlfc = obj as FaseCyclusModuleDataModel;
            if (obj == null)
            {
                throw new NotImplementedException();
            }
            else
            {
                return this.FaseCyclus.CompareTo(mlfc.FaseCyclus);
            }
        }
        #endregion // IComparable
    }
}
