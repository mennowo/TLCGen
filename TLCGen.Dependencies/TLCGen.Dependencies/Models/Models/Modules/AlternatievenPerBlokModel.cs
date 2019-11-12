using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TLCGen.Models
{
    [Serializable]
    public class AlternatievenPerBlokModel
    {
        public bool ToepassenAlternatievenPerBlok { get; set; }

        [XmlArrayItem("FaseCyclusAlternatiefPerBlok")]
        public List<FaseCyclusAlternatiefPerBlokModel> AlternatievenPerBlok { get; set; }

        public AlternatievenPerBlokModel()
        {
            AlternatievenPerBlok = new List<FaseCyclusAlternatiefPerBlokModel>();
        }
    }
}
