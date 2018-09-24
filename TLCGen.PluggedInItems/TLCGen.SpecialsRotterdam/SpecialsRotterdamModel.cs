using System;
using System.Xml.Serialization;

namespace TLCGen.SpecialsRotterdam
{
    [Serializable]
    [XmlRoot(ElementName = "SpecialsRotterdam")]
    public class SpecialsRotterdamModel
    {
        #region Fields

        #endregion // Fields

        #region Properties
        
        public bool ToevoegenOVM { get; set; }
        public bool PrmLoggingTfbMax { get; set; }

        #endregion // Properties

        #region Constructor
        #endregion // Constructor
    }
}
