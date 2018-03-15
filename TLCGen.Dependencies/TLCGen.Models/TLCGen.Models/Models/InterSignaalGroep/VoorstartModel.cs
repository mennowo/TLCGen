using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    [Serializable]
    [RefersTo("FaseVan", "FaseNaar")]
    public class VoorstartModel : IInterSignaalGroepElement
    {
        #region Properties

        public string FaseVan { get; set; }
        public string FaseNaar { get; set; }
        public int VoorstartTijd { get; set; }
        public int VoorstartOntruimingstijd { get; set; }

        #endregion // Properties
    }
}
