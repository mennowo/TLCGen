using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    [Serializable]
    [RefersTo("FaseVan", "FaseNaar")]
    public class LateReleaseModel : IInterSignaalGroepElement
    {
        #region Properties

        [HasDefault(false)]
        public string FaseVan { get; set; }
        [HasDefault(false)]
        public string FaseNaar { get; set; }
        public int LateReleaseTijd { get; set; }

        #endregion // Properties
    }
}
