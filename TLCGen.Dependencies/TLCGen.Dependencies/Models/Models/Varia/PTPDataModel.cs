using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TLCGen.Models
{
    [Serializable]
    public class PTPDataModel
    {
        public bool PTPInstellingenInParameters { get; set; }
        public bool PTPAlleenTijdensControl { get; set; }

        [XmlArrayItem(ElementName = "PTPKoppeling")]
        public List<PTPKoppelingModel> PTPKoppelingen { get; set; }

        public PTPDataModel()
        {
            PTPKoppelingen = new List<PTPKoppelingModel>();
        }
    }
}
