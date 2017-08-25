using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    [Serializable]
    [RefersToSignalGroup("FaseVan", "FaseNaar")]
    public class LateReleaseModel : IInterSignaalGroepElement
    {
        #region Properties

        public string FaseVan { get; set; }
        public string FaseNaar { get; set; }
        public int LateReleaseTijd { get; set; }

        #endregion // Properties
    }
}
