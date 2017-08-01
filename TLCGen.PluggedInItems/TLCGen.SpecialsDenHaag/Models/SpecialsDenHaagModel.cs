using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TLCGen.SpecialsDenHaag.Models
{
    [Serializable]
    [XmlRoot(ElementName = "SpecialsDenHaag")]
    public class SpecialsDenHaagModel
    {
        #region Fields
        #endregion // Fields

        #region Properties

        public bool ToepassenAlternatievenPerBlok { get; set; }

        [XmlArrayItem("FaseCyclusAlternatiefPerBlok")]
        public List<FaseCyclusAlternatiefPerBlokModel> AlternatievenPerBlok { get; set; }

        #endregion // Properties

        #region Constructor

        public SpecialsDenHaagModel()
        {
            AlternatievenPerBlok = new List<FaseCyclusAlternatiefPerBlokModel>();
        }

        #endregion // Constructor
    }
}
