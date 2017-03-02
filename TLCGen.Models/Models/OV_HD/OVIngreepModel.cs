using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    [RefersToSignalGroup("FaseCyclus")]
    public class OVIngreepModel
    {
        #region Properties

        [Browsable(false)]
        public string FaseCyclus { get; set; }

        [Category("Opties")]
        public bool KAR { get; set; }
        public bool Vecom { get; set; }
        //public bool MassaDetectie { get; set; }
        [Description("Type voertuig")]
        public OVIngreepVoertuigTypeEnum Type { get; set; }

        [Category("Tijden")]
        [Description("Rijtijd ongehinderd")]
        public int RijTijdOngehinderd { get; set; }
        [Description("Rijtijd beperkt gehinderd")]
        public int RijTijdBeperktgehinderd { get; set; }
        [Description("Rijtijd gehinderd")]
        public int RijTijdGehinderd { get; set; }
        [Description("Ondermaximum")]
        public int OnderMaximum { get; set; }
        [Description("Groenbewaking")]
        public int GroenBewaking { get; set; }

        [Category("Prioriteitsopties")]
        [Description("Afkappen conflicten")]
        public bool AfkappenConflicten { get; set; }
        [Description("Afkappen conflicterend OV")]
        public bool AfkappenConflictenOV { get; set; }
        [Description("Vasthouden groen")]
        public bool VasthoudenGroen { get; set; }
        [Description("Tussendoor realiseren")]
        public bool TussendoorRealiseren { get; set; }

        [Description("Prioriteit voor alle lijnen")]
        public bool AlleLijnen { get; set; }

        [Browsable(false)]
        [IOElement("vc", BitmappedItemTypeEnum.Uitgang, "FaseCyclus")]
        public BitmapCoordinatenDataModel OVInmeldingBitmapData { get; set; }

        [Browsable(false)]
        [IOElement("kar_dummy_in", BitmappedItemTypeEnum.Ingang, "FaseCyclus", "KAR")]
        public BitmapCoordinatenDataModel OVKARDummyInmeldingBitmapData { get; set; }

        [Browsable(false)]
        [IOElement("kar_dummy_uit", BitmappedItemTypeEnum.Ingang, "FaseCyclus", "KAR")]
        public BitmapCoordinatenDataModel OVKARDummyUitmeldingBitmapData { get; set; }

        [Browsable(false)]
        [IOElement("vecom_dummy_in", BitmappedItemTypeEnum.Ingang, "FaseCyclus", "Vecom")]
        public BitmapCoordinatenDataModel OVVecomDummyInmeldingBitmapData { get; set; }

        [Browsable(false)]
        [IOElement("vecom_dummy_uit", BitmappedItemTypeEnum.Ingang, "FaseCyclus", "Vecom")]
        public BitmapCoordinatenDataModel OVVecomDummyUitmeldingBitmapData { get; set; }

        [XmlArrayItem(ElementName = "LijnNummer")]
        public List<OVIngreepLijnNummerModel> LijnNummers { get; set; }

        //[XmlArrayItem(ElementName = "MassaDetectieMelding")]
        //public List<OVIngreepMassaDetectieMelding> MassaDetectieMeldingen { get; set; }

        #endregion // Properties

        #region Constructor

        public OVIngreepModel()
        {
            LijnNummers = new List<OVIngreepLijnNummerModel>();
            OVInmeldingBitmapData = new BitmapCoordinatenDataModel();
            OVKARDummyInmeldingBitmapData = new BitmapCoordinatenDataModel();
            OVKARDummyUitmeldingBitmapData = new BitmapCoordinatenDataModel();
            OVVecomDummyInmeldingBitmapData = new BitmapCoordinatenDataModel();
            OVVecomDummyUitmeldingBitmapData = new BitmapCoordinatenDataModel();
            //MassaDetectieMeldingen = new List<OVIngreepMassaDetectieMelding>();
        }

        #endregion // Constructor
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
