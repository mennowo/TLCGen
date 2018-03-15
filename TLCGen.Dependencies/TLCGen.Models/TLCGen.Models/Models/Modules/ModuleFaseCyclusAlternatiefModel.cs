using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    [Serializable]
    [RefersTo("FaseCyclus")]
    public class ModuleFaseCyclusAlternatiefModel
    {
        #region Properties

        public string FaseCyclus { get; set; }
        public int AlternatieveGroenTijd { get; set; }

        #endregion // Properties
    }
}
