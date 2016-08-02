using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Enumerations;
using TLCGen.Helpers.Settings;
using TLCGen.Models.Settings;

namespace TLCGen.Models
{
    public class ControllerDataModel : ModelBase
    {
        #region Fields

        #endregion // Fields

        #region Properties

        public string Naam { get; set; }
        public string Stad { get; set; }
        public string Straat1 { get; set; }
        public string Straat2 { get; set; }
        public CCOLVersieEnum CCOLVersie { get; set; }
        public KWCTypeEnum KWCType { get; set; }
        public VLOGTypeEnum VLOGType { get; set; }

        public ApplicationSettings Instellingen { get; set; }

        #endregion // Properties

        #region Constructor

        public ControllerDataModel() : base()
        {
            Instellingen = new ApplicationSettings();
        }

        #endregion // Constructor
    }
}
