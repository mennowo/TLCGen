using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TLCGen.Messaging.Messages;

namespace TLCGen.SpecialsRotterdam
{
    [Serializable]
    [XmlRoot(ElementName = "SpecialsRotterdam")]
    public class SpecialsRotterdamModel
    {
        #region Fields

        #endregion // Fields

        #region Properties

        public bool ToepassenAFM { get; set; }
        public bool PrmLoggingTfbMax { get; set; }

        #endregion // Properties

        #region Constructor
        #endregion // Constructor
    }
}
