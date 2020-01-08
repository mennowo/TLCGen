using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    [RefersTo(TLCGenObjectTypeEnum.Fase, nameof(FaseCyclus))]
    public class PrioIngreepModel : IComparable
    {
        #region Properties

        [Browsable(false)]
        [HasDefault(false)]
        public string FaseCyclus { get; set; }

        //[Obsolete("This property has been deprecated: use Meldingen instead.")]
        public bool KAR { get; set; }
        //[Obsolete("This property has been deprecated: use Meldingen instead.")]
        public bool Vecom { get; set; }
        
        public PrioIngreepVoertuigTypeEnum Type { get; set; }
        public NooitAltijdAanUitEnum VersneldeInmeldingKoplus { get; set; }
        public bool NoodaanvraagKoplus { get; set; }
        public bool KoplusKijkNaarWisselstand { get; set; }
        [RefersTo(TLCGenObjectTypeEnum.Detector)]
        [HasDefault(false)]
        public string Koplus { get; set; }

        public int RijTijdOngehinderd { get; set; }
        public int RijTijdBeperktgehinderd { get; set; }
        public int RijTijdGehinderd { get; set; }
        public int OnderMaximum { get; set; }
        public int GroenBewaking { get; set; }
        public int BlokkeertijdNaPrioIngreep { get; set; }
        public int BezettijdPrioGehinderd { get; set; }
        public int MinimaleRoodtijd { get; set; }

        public bool AfkappenConflicten { get; set; }
        public bool AfkappenConflictenPrio { get; set; }
        public bool VasthoudenGroen { get; set; }
        public bool TussendoorRealiseren { get; set; }
        public bool CheckLijnNummer { get; set; }
        public bool CheckWagenNummer { get; set; }
        public bool CheckRitCategorie { get; set; }
        public bool AlleLijnen { get; set; }
        public bool AlleRitCategorien { get; set; }

        public NooitAltijdAanUitEnum GeconditioneerdePrioriteit { get; set; }
        public int GeconditioneerdePrioTeVroeg { get; set; }
        public int GeconditioneerdePrioOpTijd { get; set; }
        public int GeconditioneerdePrioTeLaat { get; set; }

        public HalfstarPrioIngreepModel HalfstarIngreepData { get; set; }

        [HasDefault(false)]
        [Browsable(false)]
        public bool HasGeconditioneerdePrioriteit => GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit;

        [HasDefault(false)]
        [Browsable(false)]
        public string FaseCyclusType => FaseCyclus + Type.ToString();

        [Browsable(false)]
        [IOElement("vc", BitmappedItemTypeEnum.Uitgang, "FaseCyclusType")]
        public BitmapCoordinatenDataModel PrioInmeldingBitmapData { get; set; }

        [Browsable(false)]
        [IOElement("tv", BitmappedItemTypeEnum.Uitgang, "FaseCyclusType", "HasGeconditioneerdePrioriteit")]
        public BitmapCoordinatenDataModel GeconditioneerdePrioTeVroegBitmapData { get; set; }

        [Browsable(false)]
        [IOElement("ot", BitmappedItemTypeEnum.Uitgang, "FaseCyclusType", "HasGeconditioneerdePrioriteit")]
        public BitmapCoordinatenDataModel GeconditioneerdePrioOpTijdBitmapData { get; set; }

        [Browsable(false)]
        [IOElement("tl", BitmappedItemTypeEnum.Uitgang, "FaseCyclusType", "HasGeconditioneerdePrioriteit")]
        public BitmapCoordinatenDataModel GeconditioneerdePrioTeLaatBitmapData { get; set; }

        [XmlArrayItem(ElementName = "LijnNummer")]
        public List<OVIngreepLijnNummerModel> LijnNummers { get; set; }

        public PrioIngreepMeldingenDataModel MeldingenData { get; set; }

        #endregion // Properties

        #region IComparable

        public int CompareTo(object obj)
        {
	        if (!(obj is PrioIngreepModel ov2))
            {
                throw new InvalidCastException();
            }
	        return String.Compare(this.FaseCyclus, ov2.FaseCyclus, StringComparison.Ordinal);
        }

        #endregion // IComparable

        #region Constructor

        public PrioIngreepModel()
        {
            LijnNummers = new List<OVIngreepLijnNummerModel>();
            Koplus = "NG";
            PrioInmeldingBitmapData = new BitmapCoordinatenDataModel();
            GeconditioneerdePrioTeVroegBitmapData = new BitmapCoordinatenDataModel();
            GeconditioneerdePrioOpTijdBitmapData = new BitmapCoordinatenDataModel();
            GeconditioneerdePrioTeLaatBitmapData = new BitmapCoordinatenDataModel();
            MeldingenData = new PrioIngreepMeldingenDataModel();
            HalfstarIngreepData = new HalfstarPrioIngreepModel();
        }

        #endregion // Constructor
    }
}
