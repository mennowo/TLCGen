using System;
using System.Xml.Serialization;

namespace TLCGen.Specificator
{
    [Serializable]
    [XmlRoot(ElementName = "Specificator")]
    public class SpecificatorDataModel
    {
        public string Organisatie { get; set; }
        public string Straat { get; set; }
        public string Postcode { get; set; }
        public string Stad { get; set; }
        public string TelefoonNummer { get; set; }
        public string EMail { get; set; }
        public string Website { get; set; }
    }
}
