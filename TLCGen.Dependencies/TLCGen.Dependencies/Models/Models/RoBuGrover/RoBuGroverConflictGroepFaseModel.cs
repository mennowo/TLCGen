using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    [Serializable]
    [RefersTo("FaseCyclus")]
    public class RoBuGroverConflictGroepFaseModel
    {
        #region Properties

        [HasDefault(false)]
        public string FaseCyclus { get; set; }

        #endregion // Properties
    }
}