using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class PrioIngreepMeldingenDataModel
    {
        #region Properties

        [XmlArrayItem("Inmelding")]
        public List<PrioIngreepInUitMeldingModel> Inmeldingen { get; set; }
        [XmlArrayItem("Uitmelding")]
        public List<PrioIngreepInUitMeldingModel> Uitmeldingen { get; set; }

        [Browsable(false)]
        [RefersTo(TLCGenObjectTypeEnum.Input)]
        [HasDefault(false)]
        public string Wissel1Input { get; set; }
        [Browsable(false)]
        [RefersTo(TLCGenObjectTypeEnum.Detector)]
        [HasDefault(false)]
        public string Wissel2Detector { get; set; }
        [Browsable(false)]
        public bool Wissel1 { get; set; }
        [Browsable(false)]
        public PrioIngreepInUitDataWisselTypeEnum Wissel1Type { get; set; }
        [Browsable(false)]
        public bool Wissel1InputVoorwaarde { get; set; }

        [Browsable(false)]
        [RefersTo(TLCGenObjectTypeEnum.Input)]
        [HasDefault(false)]
        public string Wissel2Input { get; set; }
        [Browsable(false)]
        [RefersTo(TLCGenObjectTypeEnum.Detector)]
        [HasDefault(false)]
        public string Wissel1Detector { get; set; }
        [Browsable(false)]
        public bool Wissel2 { get; set; }
        [Browsable(false)]
        public PrioIngreepInUitDataWisselTypeEnum Wissel2Type { get; set; }
        [Browsable(false)]
        public bool Wissel2InputVoorwaarde { get; set; }
        
        public bool AntiJutterVoorAlleInmeldingen { get; set; }
        public int AntiJutterTijdVoorAlleInmeldingen { get; set; }
        public bool AntiJutterVoorAlleUitmeldingen { get; set; }
        public int AntiJutterTijdVoorAlleUitmeldingen { get; set; }

        #endregion // Properties

        #region Constructor

        public PrioIngreepMeldingenDataModel()
        {
            Inmeldingen = new List<PrioIngreepInUitMeldingModel>();
            Uitmeldingen = new List<PrioIngreepInUitMeldingModel>();
        }

        #endregion // Constructor
    }
}
