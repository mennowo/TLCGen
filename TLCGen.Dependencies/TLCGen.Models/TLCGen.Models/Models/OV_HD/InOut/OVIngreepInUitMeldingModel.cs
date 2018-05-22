using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using TLCGen.Helpers;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class OVIngreepMeldingenDataModel
    {
        [XmlArrayItem("Inmelding")]
        public List<OVIngreepInUitMeldingModel> Inmeldingen { get; set; }
        [XmlArrayItem("Uitmelding")]
        public List<OVIngreepInUitMeldingModel> Uitmeldingen { get; set; }

        public OVIngreepInUitDataWisselType Wissel1Type { get; set; }
        public OVIngreepInUitDataWisselType Wissel2Type { get; set; }

        public bool Wissel1 { get; set; }
        [RefersTo]
        public string Wissel1Input { get; set; }
        public bool Wissel1InputVoorwaarde { get; set; }
        [RefersTo]
        public string Wissel1Detector { get; set; }

        public bool Wissel2 { get; set; }
        [RefersTo]
        public string Wissel2Input { get; set; }
        public bool Wissel2InputVoorwaarde { get; set; }
        [RefersTo]
        public string Wissel2Detector { get; set; }

        public bool AntiJutterVoorAlleInmeldingen { get; set; }
        public int AntiJutterTijdVoorAlleInmeldingen { get; set; }
        public bool AntiJutterVoorAlleUitmeldingen { get; set; }
        public int AntiJutterTijdVoorAlleUitmeldingen { get; set; }

        public OVIngreepMeldingenDataModel()
        {
            Inmeldingen = new List<OVIngreepInUitMeldingModel>();
            Uitmeldingen = new List<OVIngreepInUitMeldingModel>();
        }
    }

    [Serializable]
    public class OVIngreepInUitMeldingModel
    {
        //public string Omschrijving { get; set; }
        public OVIngreepInUitMeldingType InUit { get; set; }
        //[XmlArrayItem("Voorwaarde")]
        //public List<OVIngreepInUitMeldingVoorwaardeModel> Voorwaarden { get; set; }
        public bool KijkNaarWisselStand { get; set; }
        public bool AlleenIndienGeenInmelding { get; set; }
        public bool AntiJutterTijdToepassen { get; set; }
        public int AntiJutterTijd { get; set; }

        public OVIngreepMassaDetectieMeldingVoorwaardeTypeEnum Type { get; set; }
        public string RelatedInput1 { get; set; }
        public OVIngreepInUitMeldingVoorwaardeInputTypeEnum RelatedInput1Type { get; set; }
        public bool TweedeInput { get; set; }
        public bool OpvangStoring { get; set; }
        public OVIngreepInUitMeldingVoorwaardeInputOmgangMetStoringTypeEnum OpvangStoring2Detectors { get; set; }
        public string RelatedInput2 { get; set; }
        public OVIngreepInUitMeldingVoorwaardeInputTypeEnum RelatedInput2Type { get; set; }

        public OVIngreepInUitMeldingModel()
        {
            //Voorwaarden = new List<OVIngreepInUitMeldingVoorwaardeModel>();
        }
    }

    [Serializable]
    [RefersTo("RelatedInput")]
    public class OVIngreepInUitMeldingVoorwaardeModel
    {
        public string Omschrijving { get; set; }
        public OVIngreepMassaDetectieMeldingVoorwaardeTypeEnum Type { get; set; }
        public string RelatedInput1 { get; set; }
        public OVIngreepInUitMeldingVoorwaardeInputTypeEnum RelatedInput1Type { get; set; }
        public bool TweedeInput { get; set; }
        public bool OpvangStoring { get; set; }
        public OVIngreepInUitMeldingVoorwaardeInputOmgangMetStoringTypeEnum OpvangStoring2Detectors { get; set; }
        public string RelatedInput2 { get; set; }
        public OVIngreepInUitMeldingVoorwaardeInputTypeEnum RelatedInput2Type { get; set; }

        public OVIngreepInUitMeldingVoorwaardeModel()
        {

        }
    }

    //public enum OVIngreepInUitMeldingDetectorVoorwaadeTypeEnum
    //{
    //    Start,
    //    Bezet,
    //    Einde,
    //    Storing
    //}
    //
    //[Serializable]
    //public class OVIngreepInUitMeldingVoorwaadeModel
    //{
    //    public OVIngreepInUitMeldingDetectorVoorwaadeTypeEnum Detector1Type { get; set; }
    //    public string Detector1 { get; set; }
    //    public OVIngreepInUitMeldingDetectorVoorwaadeTypeEnum Detector2Type { get; set; }
    //    public string Detector2 { get; set; }
    //    public bool AntiJutter { get; set; }
    //    public int Juttertijd { get; set; }
    //    public int NogGeenInmelding { get; set; }
    //    public bool WisselAanwezig { get; set; }
    //    public string WisselDetector { get; set; }
    //}
    //
    //public enum OVIngreepInUitMeldingTypeEnum
    //{
    //    Inmelding, Uitmelding
    //}
    //
    //[Serializable]
    //public class OVIngreepInUitMeldingModel
    //{
    //
    //
    //    public List<OVIngreepInUitMeldingVoorwaadeModel> Voorwaarden { get; set; }
    //
    //    public OVIngreepInUitMeldingModel()
    //    {
    //        Voorwaarden = new List<OVIngreepInUitMeldingVoorwaadeModel>();
    //    }
    //}
}
