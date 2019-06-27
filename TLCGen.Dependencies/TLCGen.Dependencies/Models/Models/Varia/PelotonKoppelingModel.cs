using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models.Enumerations
{
    public enum PelotonKoppelingType
    {
        DenHaag,
        RHDHV
    }
}

namespace TLCGen.Models
{
    [Serializable]
    public class PelotonKoppelingModel
    {
        [HasDefault(false)]
        public string KruisingNaam { get; set; }

        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        [HasDefault(false)]
        public string GekoppeldeSignaalGroep { get; set; }

        public PelotonKoppelingType Type { get; set; }
        public int Meetperiode { get; set; }
        public int MaximaalHiaat { get; set; }
        public int MinimaalAantalVoertuigen { get; set; }
        public int Verschuiving { get; set; }
        public int TijdTotAanvraag { get; set; }
        public int TijdTotRetourWachtgroen { get; set; }
        public int TijdRetourWachtgroen { get; set; }
        public int MaxTijdToepassenRetourWachtgroen { get; set; }
        public NooitAltijdAanUitEnum ToepassenAanvraag { get; set; }
        public NooitAltijdAanUitEnum ToepassenMeetkriterium { get; set; }
        public NooitAltijdAanUitEnum ToepassenRetourWachtgroen { get; set; }
        public bool AutoIngangsSignalen { get; set; }
        public int IngangsSignaalFG { get; set; }

        [RefersTo(TLCGenObjectTypeEnum.PTPKruising)]
        [HasDefault(false)]
        public string PTPKruising { get; set; }

        public HalfstarGekoppeldWijzeEnum KoppelWijze { get; set; }

        public PelotonKoppelingRichtingEnum Richting { get; set; }

        [XmlArray(ElementName = "PelotonKoppelingDetector")]
        public List<PelotonKoppelingDetectorModel> Detectoren { get; set; }

	    [IOElement("pelin", BitmappedItemTypeEnum.Uitgang, "GekoppeldeSignaalGroep", "IsInkomend")]
        public BitmapCoordinatenDataModel InkomendVerklikking { get; set; }

        [Browsable(false)]
        public bool IsInkomend => Richting == PelotonKoppelingRichtingEnum.Inkomend;

        public PelotonKoppelingModel()
        {
            Detectoren = new List<PelotonKoppelingDetectorModel>();
            InkomendVerklikking = new BitmapCoordinatenDataModel();
        }
    }
}
