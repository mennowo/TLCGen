using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    public enum CCOLVersion { CCOL8 }
    public enum KWCType { Geen, KoHartog, Imtech, Swarco, Vialis }
    public enum VLOGType { Geen, Streaming, Filebased }

    public class ControllerDataModel : ModelBase
    {
        #region Fields

        #endregion // Fields

        #region Properties

        public string Naam;
        public string Stad;
        public string Straat1;
        public string Straat2;
        public CCOLVersion CCOLVersie;
        public KWCType KWCType;
        public VLOGType VLOGType;

        #endregion // Properties

        #region Constructor

        public ControllerDataModel() : base()
        {

        }

        #endregion // Constructor
    }
}
