using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class OVIngreepModel
    {
        public string FaseCyclus { get; set; }
        public bool KAR { get; set; }
        public bool Vecom { get; set; }
        public bool MassaDetectie { get; set; }
        public OVIngreepVoertuigTypeEnum Type { get; set; }
        public bool AlleLijnen { get; set; }

        public int RijTijdOngehinderd { get; set; }
        public int RijTijdBeperktgehinderd { get; set; }
        public int RijTijdGehinderd { get; set; }
        public int OnderMaximum { get; set; }
        public int GroenBewaking { get; set; }

        [XmlArrayItem(ElementName = "LijnNummer")]
        public List<string> LijnNummers { get; set; }

        //[XmlArrayItem(ElementName = "MassaDetectieMelding")]
        //public List<OVIngreepMassaDetectieMelding> MassaDetectieMeldingen { get; set; }

        public OVIngreepModel()
        {
            LijnNummers = new List<string>();
            //MassaDetectieMeldingen = new List<OVIngreepMassaDetectieMelding>();
        }
    }

    //public class OVIngreepMassaDetectieMelding
    //{
    //    OVIngreepTypeEnum Type { get; set; }
    //    public List<OVIngreepMassaDetectieMeldingVoorwaardenSet> VoorwaardenSets { get; set; }
    //}
    //
    //public class OVIngreepMassaDetectieMeldingVoorwaardenSet
    //{
    //    public int Rangorde { get; set; }
    //
    //    [XmlArrayItem(ElementName = "Voorwaarde")]
    //    public List<OVIngreepMassaDetectieMeldingVoorwaarde> Voorwaarden { get; set; }
    //}
    //
    //public class OVIngreepMassaDetectieMeldingVoorwaarde
    //{
    //    public OVIngreepMassaDetectieMeldingVoorwaardeTypeEnum Type { get; set; }
    //    public string Detector { get; set; }
    //}
    //
    ////[TypeConverter(typeof(EnumDescriptionTypeConverter))]
    //public enum OVIngreepTypeEnum
    //{
    //    Inmelding,
    //    Uitmelding
    //}
    //
    ////[TypeConverter(typeof(EnumDescriptionTypeConverter))]
    //public enum OVIngreepMassaDetectieMeldingVoorwaardeTypeEnum
    //{
    //    DetectorStart,
    //    DetectorBezet,
    //    DetectorEind,
    //    DetectorGeenStoring,
    //    DetectorStoring
    //}
}
