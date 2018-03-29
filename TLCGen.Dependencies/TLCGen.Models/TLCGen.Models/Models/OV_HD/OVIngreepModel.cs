using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    [RefersTo(nameof(FaseCyclus))]
    public class OVIngreepModel : IComparable
    {
        #region Properties

        [Browsable(false)]
        public string FaseCyclus { get; set; }

        //[Obsolete("This property has been deprecated: use Meldingen instead.")]
        public bool KAR { get; set; }
        //[Obsolete("This property has been deprecated: use Meldingen instead.")]
        public bool Vecom { get; set; }

        public List<OVIngreepMeldingModel> Meldingen { get; set; }
        public int UitmeldFilterTijd { get; set; }
        
        public bool Wissel { get; set; }
        public OVIngreepWisselTypeEnum WisselType { get; set; }
        [RefersTo]
        public string WisselStandInput { get; set; }
        public bool WisselStandVoorwaarde { get; set; }

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
        public bool CheckLijnNummer { get; set; }
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

            Meldingen = new List<OVIngreepMeldingModel>();
        }

        #endregion // Constructor
    }
}
