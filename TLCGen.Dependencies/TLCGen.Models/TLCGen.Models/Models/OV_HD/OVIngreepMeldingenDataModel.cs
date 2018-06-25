using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class OVIngreepMeldingenDataModel
    {
        #region Properties

        [XmlArrayItem("Inmelding")]
        public List<OVIngreepInUitMeldingModel> Inmeldingen { get; set; }
        [XmlArrayItem("Uitmelding")]
        public List<OVIngreepInUitMeldingModel> Uitmeldingen { get; set; }

        [RefersTo]
        [HasDefault(false)]
        public string Wissel1Input { get; set; }
        [RefersTo]
        [HasDefault(false)]
        public string Wissel2Detector { get; set; }
        public bool Wissel1 { get; set; }
        public OVIngreepInUitDataWisselTypeEnum Wissel1Type { get; set; }
        public bool Wissel1InputVoorwaarde { get; set; }

        [RefersTo]
        [HasDefault(false)]
        public string Wissel2Input { get; set; }
        [RefersTo]
        [HasDefault(false)]
        public string Wissel1Detector { get; set; }
        public bool Wissel2 { get; set; }
        public OVIngreepInUitDataWisselTypeEnum Wissel2Type { get; set; }
        public bool Wissel2InputVoorwaarde { get; set; }
        
        public bool AntiJutterVoorAlleInmeldingen { get; set; }
        public int AntiJutterTijdVoorAlleInmeldingen { get; set; }
        public bool AntiJutterVoorAlleUitmeldingen { get; set; }
        public int AntiJutterTijdVoorAlleUitmeldingen { get; set; }

        #endregion // Properties

        #region Constructor

        public OVIngreepMeldingenDataModel()
        {
            Inmeldingen = new List<OVIngreepInUitMeldingModel>();
            Uitmeldingen = new List<OVIngreepInUitMeldingModel>();
        }

        #endregion // Constructor
    }
}
