using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    [Serializable]
    [RefersToSignalGroup("FaseCyclus")]
    public class RoBuGroverConflictGroepFaseModel
    {
        #region Properties

        public string FaseCyclus { get; set; }

        #endregion // Properties
    }
}