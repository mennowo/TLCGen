using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    [HasKoppelSignalen]
    public class PelotonKoppelingModel : IHaveKoppelSignalen
    {
        #region Properties

        [HasDefault(false)]
        [RefersTo(TLCGenObjectTypeEnum.PelotonKoppeling)]
        [ModelName(TLCGenObjectTypeEnum.PelotonKoppeling)]
        public string KoppelingNaam { get; set; }
        
        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        [HasDefault(false)]
        public string GekoppeldeSignaalGroep { get; set; }

        public PelotonKoppelingTypeEnum Type { get; set; }
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

        public bool IsIntern { get; set; }
        [RefersTo(TLCGenObjectTypeEnum.PelotonKoppeling)]
        [HasDefault(false)]
        public string GerelateerdePelotonKoppeling { get; set; }

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

        [Browsable(false)]
        public List<KoppelSignaalModel> KoppelSignalen { get; set; }

        #endregion // Properties

        #region Constructor

        public PelotonKoppelingModel()
        {
            Detectoren = new List<PelotonKoppelingDetectorModel>();
            InkomendVerklikking = new BitmapCoordinatenDataModel();
            KoppelSignalen = new List<KoppelSignaalModel>();
        }

        #endregion // Constructor

        #region IHaveKoppelSignalen

        public List<KoppelSignaalModel> UpdateKoppelSignalen()
        {
            var signalen = new List<KoppelSignaalModel>();
            if (IsIntern) return signalen;
            var id = 1;
            switch (Type)
            {
                case PelotonKoppelingTypeEnum.DenHaag:
                    switch (Richting)
                    {
                        case PelotonKoppelingRichtingEnum.Uitgaand:
                            signalen.Add(new KoppelSignaalModel
                            {
                                Id = id++,
                                Name = $"{KoppelingNaam}g{GekoppeldeSignaalGroep}",
                                Description = $"{KoppelingNaam} groen {GekoppeldeSignaalGroep}",
                                Koppeling = PTPKruising,
                                Richting = KoppelSignaalRichtingEnum.Uit
                            });
                            foreach (var d in Detectoren)
                            {
                                signalen.Add(new KoppelSignaalModel
                                {
                                    Id = id++,
                                    Name = $"{KoppelingNaam}d{d.DetectorNaam}",
                                    Description = $"{KoppelingNaam} det. {d.DetectorNaam}",
                                    Koppeling = PTPKruising,
                                    Richting = KoppelSignaalRichtingEnum.Uit
                                });
                            }
                            break;
                        case PelotonKoppelingRichtingEnum.Inkomend:
                            signalen.Add(new KoppelSignaalModel
                            {
                                Id = id++,
                                Name = $"{KoppelingNaam}g{GekoppeldeSignaalGroep}",
                                Description = $"{KoppelingNaam} groen {GekoppeldeSignaalGroep}",
                                Koppeling = PTPKruising,
                                Richting = KoppelSignaalRichtingEnum.In
                            });
                            foreach (var d in Detectoren)
                            {
                                signalen.Add(new KoppelSignaalModel
                                {
                                    Id = id++,
                                    Name = $"{KoppelingNaam}d{d.DetectorNaam}",
                                    Description = $"{KoppelingNaam} det. {d.DetectorNaam}",
                                    Koppeling = PTPKruising,
                                    Richting = KoppelSignaalRichtingEnum.In
                                });
                            }
                            break;
                    }
                    break;
                case PelotonKoppelingTypeEnum.RHDHV:
                    switch (Richting)
                    {
                        case PelotonKoppelingRichtingEnum.Uitgaand:
                            signalen.Add(new KoppelSignaalModel
                            {
                                Id = id++,
                                Name = $"{KoppelingNaam}g{GekoppeldeSignaalGroep}",
                                Description = $"{KoppelingNaam} peloton {GekoppeldeSignaalGroep}",
                                Koppeling = PTPKruising,
                                Richting = KoppelSignaalRichtingEnum.Uit
                            });
                            break;
                        case PelotonKoppelingRichtingEnum.Inkomend:
                            signalen.Add(new KoppelSignaalModel
                            {
                                Id = id++,
                                Name = $"{KoppelingNaam}g{GekoppeldeSignaalGroep}",
                                Description = $"{KoppelingNaam} peloton {GekoppeldeSignaalGroep}",
                                Koppeling = PTPKruising,
                                Richting = KoppelSignaalRichtingEnum.In
                            });
                            break;
                    }
                    break;
            }
            foreach (var s in signalen)
            {
                s.Count = 0;
                s.Koppeling = PTPKruising;
            }
            foreach (var s in KoppelSignalen)
            {
                var ns = signalen.FirstOrDefault(x => x.Id != 0 && x.Id == s.Id);
                if (ns != null)
                {
                    ns.Count = s.Count;
                }
            }
            KoppelSignalen = signalen;
            return KoppelSignalen;
        }

        #endregion // IHaveKoppelSignalen
    }
}
