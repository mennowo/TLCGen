using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    [IOElement("", BitmappedItemTypeEnum.Fase, "Naam")]
    public class FaseCyclusModel : IOElementModel, IComparable, IHaveName
    {
        #region Properties
        
        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        [ModelName(TLCGenObjectTypeEnum.Fase)]
        [Browsable(false)]
        public override string Naam { get; set; }
        public override bool Dummy { get; set; }

        [Browsable(false)] [HasDefault(false)] public TLCGenObjectTypeEnum ObjectType => TLCGenObjectTypeEnum.Fase;

        [Browsable(false)]
        public FaseTypeEnum Type { get; set; }

        public int TFG { get; set; }
        public int TGG { get; set; }
        public int TGG_min { get; set; }
        public int TRG { get; set; }
        public int TRG_min { get; set; }
        public int TGL { get; set; }
        public int TGL_min { get; set; }
        public int Kopmax { get; set; }
        [Browsable(false)]
        [HasDefault(false)]
        [NoTemplateApplicable]
        public bool PrioIngreep { get; set; }
        [Browsable(false)]
        [HasDefault(false)]
        [NoTemplateApplicable]
        public bool PrioIngreepGeconditioneerd { get; set; }
        [Browsable(false)]
        [HasDefault(false)]
        [NoTemplateApplicable]
        public bool HDIngreep { get; set; }
        public NooitAltijdAanUitEnum VasteAanvraag { get; set; }
        public NooitAltijdAanUitEnum Wachtgroen { get; set; }
        public NooitAltijdAanUitEnum Meeverlengen { get; set; }
        public MeeVerlengenTypeEnum MeeverlengenType { get; set; }
        public bool MeeverlengenTypeInstelbaarOpStraat { get; set; }
        public int? MeeverlengenVerschil { get; set; }
        public bool UitgesteldeVasteAanvraag { get; set; }
        public int UitgesteldeVasteAanvraagTijdsduur { get; set; }
        public bool AlternatiefBinnenDrukknop { get; set; }
        public int? AantalRijstroken { get; set; }
        public bool HiaatKoplusBijDetectieStoring { get; set; }
        public int? VervangendHiaatKoplus { get; set; }
        public bool AanvraagBijDetectieStoring { get; set; }
        public bool AanvraagBijDetectieStoringVertraagd { get; set; }
        public int AanvraagBijDetectieStoringVertraging { get; set; }
        public bool AanvraagBijDetectieStoringKoplusKnop { get; set; }
        public bool AanvraagBijDetectieStoringKopLang { get; set; }
        public bool PercentageGroenBijDetectieStoring { get; set; }
        public int? PercentageGroen { get; set; }
        public int VeiligheidsGroenMaximaal { get; set; }
        public bool WachttijdVoorspeller { get; set; }
        public bool ToepassenMK2 { get; set; }
        public NooitAltijdAanUitEnum SchoolIngreep { get; set; }
        public int SchoolIngreepMaximumGroen { get; set; }
        public int SchoolIngreepBezetTijd { get; set; }
        public NooitAltijdAanUitEnum SeniorenIngreep { get; set; }
        public int SeniorenIngreepBezetTijd { get; set; }
        public int SeniorenIngreepExtraGroenPercentage { get; set; }

        [XmlArrayItem(ElementName = "Detector")]
        public List<DetectorModel> Detectoren { get; set; }

        [IOElement("vc", BitmappedItemTypeEnum.Uitgang, nameof(Naam), nameof(PrioIngreep))]
        public BitmapCoordinatenDataModel PrioIngreepBitmapData { get; set; }

        [Browsable(false)]
        [IOElement("tv", BitmappedItemTypeEnum.Uitgang, nameof(Naam), nameof(PrioIngreepGeconditioneerd))]
        public BitmapCoordinatenDataModel GeconditioneerdePrioTeVroegBitmapData { get; set; }

        [Browsable(false)]
        [IOElement("ot", BitmappedItemTypeEnum.Uitgang, nameof(Naam), nameof(PrioIngreepGeconditioneerd))]
        public BitmapCoordinatenDataModel GeconditioneerdePrioOpTijdBitmapData { get; set; }

        [Browsable(false)]
        [IOElement("tl", BitmappedItemTypeEnum.Uitgang, nameof(Naam), nameof(PrioIngreepGeconditioneerd))]
        public BitmapCoordinatenDataModel GeconditioneerdePrioTeLaatBitmapData { get; set; }

        [IOElement("wtv", BitmappedItemTypeEnum.Uitgang, nameof(Naam), nameof(WachttijdVoorspeller))]
        public BitmapCoordinatenDataModel WachttijdVoorspellerBitmapData { get; set; }

        [XmlIgnore]
        [HasDefault(false)]
        [Browsable(false)]
        public bool HasWachttijdVoorspellerBus { get; set; }

        [IOElement("wtvbus", BitmappedItemTypeEnum.Uitgang, nameof(Naam), nameof(HasWachttijdVoorspellerBus))]
        public BitmapCoordinatenDataModel WachttijdVoorspellerBusBitmapData { get; set; }

        [IOElement("wtv0", BitmappedItemTypeEnum.Uitgang, nameof(Naam), nameof(WachttijdVoorspeller))]
        public BitmapCoordinatenDataModel WachttijdVoorspellerBitmapData0 { get; set; }

        [IOElement("wtv1", BitmappedItemTypeEnum.Uitgang, nameof(Naam), nameof(WachttijdVoorspeller))]
        public BitmapCoordinatenDataModel WachttijdVoorspellerBitmapData1 { get; set; }

        [IOElement("wtv2", BitmappedItemTypeEnum.Uitgang, nameof(Naam), nameof(WachttijdVoorspeller))]
        public BitmapCoordinatenDataModel WachttijdVoorspellerBitmapData2 { get; set; }
        
        [IOElement("wtv3", BitmappedItemTypeEnum.Uitgang, nameof(Naam), nameof(WachttijdVoorspeller))]
        public BitmapCoordinatenDataModel WachttijdVoorspellerBitmapData3 { get; set; }
        
        [IOElement("wtv4", BitmappedItemTypeEnum.Uitgang, nameof(Naam), nameof(WachttijdVoorspeller))]
        public BitmapCoordinatenDataModel WachttijdVoorspellerBitmapData4 { get; set; }

        [XmlArrayItem(ElementName = "HardMeeverlengFaseCyclus")]
        public List<HardMeeverlengenFaseCyclusModel> HardMeeverlengenFaseCycli { get; set; }

        [HasDefault(false)]
        [Browsable(false)]
        public int NameAsInt
        {
            get
            {
                if (int.TryParse(Naam, out var iName))
                {
                    return iName;
                }

                return -1;
            }
        }

        #endregion // Properties

        #region IComparable

        public int CompareTo(object obj)
        {
            if(obj is FaseCyclusModel)
            {
                var s1 = (obj as FaseCyclusModel).Naam;
                var s2 = this.Naam;
                if (s1.Length < s2.Length) s1 = s1.PadLeft(s2.Length, '0');
                else if (s2.Length < s1.Length) s2 = s2.PadLeft(s1.Length, '0');

                return s2.CompareTo(s1);
            }
            return 0;
        }

        #endregion // IComparable

        public override string ToString()
        {
            return Naam;
        }

        #region Constructor

        public FaseCyclusModel() : base()
        {
            Detectoren = new List<DetectorModel>();
            HardMeeverlengenFaseCycli = new List<HardMeeverlengenFaseCyclusModel>();
            PrioIngreepBitmapData = new BitmapCoordinatenDataModel();
            GeconditioneerdePrioOpTijdBitmapData = new BitmapCoordinatenDataModel();
            GeconditioneerdePrioTeLaatBitmapData = new BitmapCoordinatenDataModel();
            GeconditioneerdePrioTeVroegBitmapData = new BitmapCoordinatenDataModel();
            WachttijdVoorspellerBitmapData = new BitmapCoordinatenDataModel();
            WachttijdVoorspellerBusBitmapData = new BitmapCoordinatenDataModel();
            WachttijdVoorspellerBitmapData0 = new BitmapCoordinatenDataModel();
            WachttijdVoorspellerBitmapData1 = new BitmapCoordinatenDataModel();
            WachttijdVoorspellerBitmapData2 = new BitmapCoordinatenDataModel();
            WachttijdVoorspellerBitmapData3 = new BitmapCoordinatenDataModel();
            WachttijdVoorspellerBitmapData4 = new BitmapCoordinatenDataModel();
            AantalRijstroken = 1;
        }

        #endregion // Constructor
    }
}
