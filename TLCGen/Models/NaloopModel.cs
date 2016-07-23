using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    public enum NaloopType { StartGroen, EindeGroen }

    public class NaloopModel : ModelBase
    {
        #region Fields

        #endregion // Fields

        #region Properties

        public int FaseFrom { get; set; }
        public int FaseTo { get; set; }
        public int Detector { get; set; }
        public int Tijd { get; set; }
        public int TijdDetector { get; set; }
        public int TijdVastGroen { get; set; }
        public int TijdDetectorVastGroen { get; set; }

        #endregion // Properties

        #region Constructor

        public NaloopModel() : base()
        {

        }

        #endregion // Constructor
    }
}
