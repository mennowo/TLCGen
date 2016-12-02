using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;
using TLCGen.Models.Settings;

namespace TLCGen.Models
{
    [Serializable]
    public class ControllerDataModel
    {
        #region Fields

        #endregion // Fields

        #region Properties

        public string Naam { get; set; }
        public string Stad { get; set; }
        public string Straat1 { get; set; }
        public string Straat2 { get; set; }
        public string BitmapNaam { get; set; }

        public CCOLVersieEnum CCOLVersie { get; set; }
        public KWCTypeEnum KWCType { get; set; }
        public VLOGTypeEnum VLOGType { get; set; }
        public TLCGenVersieEnum TLCGenVersie { get; set; }

        public bool GarantieOntruimingsTijden { get; set; }
        public bool ExtraMeeverlengenInWG { get; set; }
        public GroentijdenTypeEnum TypeGroentijden { get; set; }
        public AansturingWaitsignalenEnum AansturingWaitsignalen { get; set; }

        public List<VersieModel> Versies { get; set; }

        #endregion // Properties

        #region Constructor

        public ControllerDataModel() : base()
        {
            Versies = new List<VersieModel>();
        }

        #endregion // Constructor
    }
}
