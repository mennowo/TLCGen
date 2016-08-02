using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Enumerations;

namespace TLCGen.Models
{

    public class DetectorModel : ModelBase
    {
        #region Fields

        #endregion // Fields

        #region Properties

        public string Naam { get; set; }
        public string Define { get; set; }
        public int TDB { get; set; }
        public int TDH { get; set; }
        public int TOG { get; set; }
        public int TBG { get; set; }
        public int TFL { get; set; }
        public int CFL { get; set; }

        DetectorTypeEnum Type { get; set; }
        DetectorAanvraagTypeEnum Aanvraag { get; set; }
        DetectorVerlengenTypeEnum Verlengen { get; set; }

        #endregion // Properties

        #region Constructor

        public DetectorModel() : base()
        {

        }

        #endregion // Constructor
    }
}
