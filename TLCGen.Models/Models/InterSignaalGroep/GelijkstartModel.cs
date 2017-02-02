using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    [Serializable]
    public class GelijkstartModel : IInterSignaalGroepElement
    {
        #region Properties

        public string FaseVan { get; set; }
        public string FaseNaar { get; set; }
        public int GelijkstartOntruimingstijdFaseVan { get; set; }
        public int GelijkstartOntruimingstijdFaseNaar { get; set; }
        public bool DeelConflict { get; set; }

        #endregion // Properties
    }
}
