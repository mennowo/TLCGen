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
        [HasDefault(false)]
        public string FaseCyclus { get; set; }

        //[Obsolete("This property has been deprecated: use Meldingen instead.")]
        public bool KAR { get; set; }
        //[Obsolete("This property has been deprecated: use Meldingen instead.")]
        public bool Vecom { get; set; }
        
        public OVIngreepVoertuigTypeEnum Type { get; set; }
        public NooitAltijdAanUitEnum VersneldeInmeldingKoplus { get; set; }
        public bool NoodaanvraagKoplus { get; set; }
        public bool KoplusKijkNaarWisselstand { get; set; }
        [RefersTo]
        [HasDefault(false)]
        public string Koplus { get; set; }

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

        public NooitAltijdAanUitEnum GeconditioneerdePrioriteit { get; set; }
        public int GeconditioneerdePrioTeVroeg { get; set; }
        public int GeconditioneerdePrioOpTijd { get; set; }
        public int GeconditioneerdePrioTeLaat { get; set; }

        [HasDefault(false)]
        [Browsable(false)]
        public bool HasGeconditioneerdePrioriteit => GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit;

        [Browsable(false)]
        [IOElement("vc", BitmappedItemTypeEnum.Uitgang, "FaseCyclus")]
        public BitmapCoordinatenDataModel OVInmeldingBitmapData { get; set; }

        [Browsable(false)]
        [IOElement("tv", BitmappedItemTypeEnum.Uitgang, "FaseCyclus", "HasGeconditioneerdePrioriteit")]
        public BitmapCoordinatenDataModel GeconditioneerdePrioTeVroegBitmapData { get; set; }

        [Browsable(false)]
        [IOElement("ot", BitmappedItemTypeEnum.Uitgang, "FaseCyclus", "HasGeconditioneerdePrioriteit")]
        public BitmapCoordinatenDataModel GeconditioneerdePrioOpTijdBitmapData { get; set; }

        [Browsable(false)]
        [IOElement("tl", BitmappedItemTypeEnum.Uitgang, "FaseCyclus", "HasGeconditioneerdePrioriteit")]
        public BitmapCoordinatenDataModel GeconditioneerdePrioTeLaatBitmapData { get; set; }

        [Browsable(false)]
        public DetectorModel DummyKARInmelding { get; set; }
        [Browsable(false)]
        public DetectorModel DummyKARUitmelding { get; set; }

        [XmlArrayItem(ElementName = "LijnNummer")]
        public List<OVIngreepLijnNummerModel> LijnNummers { get; set; }

        public OVIngreepMeldingenDataModel MeldingenData { get; set; }

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
            GeconditioneerdePrioTeVroegBitmapData = new BitmapCoordinatenDataModel();
            GeconditioneerdePrioOpTijdBitmapData = new BitmapCoordinatenDataModel();
            GeconditioneerdePrioTeLaatBitmapData = new BitmapCoordinatenDataModel();
            MeldingenData = new OVIngreepMeldingenDataModel();
        }

        #endregion // Constructor
    }
}
