using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    [RefersToSignalGroup("FaseCyclus")]
    public class OVIngreepModel : IComparable
    {
        #region Properties

        [Browsable(false)]
        public string FaseCyclus { get; set; }

        public bool KAR { get; set; }
        public bool Vecom { get; set; }
        //public bool MassaDetectie { get; set; }
        public OVIngreepVoertuigTypeEnum Type { get; set; }
        public NooitAltijdAanUitEnum VersneldeInmeldingKoplus { get; set; }

        public int RijTijdOngehinderd { get; set; }
        public int RijTijdBeperktgehinderd { get; set; }
        public int RijTijdGehinderd { get; set; }
        public int OnderMaximum { get; set; }
        public int GroenBewaking { get; set; }

        public bool AfkappenConflicten { get; set; }
        public bool AfkappenConflictenOV { get; set; }
        public bool VasthoudenGroen { get; set; }
        public bool TussendoorRealiseren { get; set; }
        public bool AlleLijnen { get; set; }

        [Browsable(false)]
        [IOElement("vc", BitmappedItemTypeEnum.Uitgang, "FaseCyclus")]
        public BitmapCoordinatenDataModel OVInmeldingBitmapData { get; set; }

        [Browsable(false)]
        public DetectorModel DummyKARInmelding { get; set; }
        [Browsable(false)]
        public DetectorModel DummyKARUitmelding { get; set; }
        [Browsable(false)]
        public DetectorModel DummyVecomInmelding { get; set; }
        [Browsable(false)]
        public DetectorModel DummyVecomUitmelding { get; set; }

        [XmlArrayItem(ElementName = "LijnNummer")]
        public List<OVIngreepLijnNummerModel> LijnNummers { get; set; }

        //[XmlArrayItem(ElementName = "MassaDetectieMelding")]
        //public List<OVIngreepMassaDetectieMelding> MassaDetectieMeldingen { get; set; }

        #endregion // Properties

        #region IComparable

        public int CompareTo(object obj)
        {
	        if (!(obj is OVIngreepModel ov2))
            {
                throw new InvalidCastException();
            }
	        return String.Compare(this.FaseCyclus, ov2.FaseCyclus, StringComparison.Ordinal);
        }

        #endregion // IComparable

        #region Constructor

        public OVIngreepModel()
        {
            LijnNummers = new List<OVIngreepLijnNummerModel>();
            OVInmeldingBitmapData = new BitmapCoordinatenDataModel();

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
