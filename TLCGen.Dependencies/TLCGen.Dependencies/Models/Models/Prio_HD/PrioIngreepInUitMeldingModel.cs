using System;
using System.ComponentModel;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class PrioIngreepInUitMeldingModel
    {
        #region Properties

        public string Naam { get; set; }

        public PrioIngreepInUitMeldingTypeEnum InUit { get; set; }
        public PrioIngreepInUitMeldingVoorwaardeTypeEnum Type { get; set; }

        #region Reguliere IO (detectie, Vecom, SD via ingang, wissels)

        [RefersTo(TLCGenObjectTypeEnum.Input, nameof(InputTLCGenType))]
        [HasDefault(false)]
        public string RelatedInput1 { get; set; }
        public PrioIngreepInUitMeldingVoorwaardeInputTypeEnum RelatedInput1Type { get; set; }

        public TLCGenObjectTypeEnum InputTLCGenType
        {
            get
            {
                switch (Type)
                {
                    case PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector:
                    case PrioIngreepInUitMeldingVoorwaardeTypeEnum.VecomViaDetector:
                        return TLCGenObjectTypeEnum.Detector;
                    case PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector:
                        return TLCGenObjectTypeEnum.SelectieveDetector;
                    default:
                        return TLCGenObjectTypeEnum.Detector;
                }
            }
        }

        public bool TweedeInput { get; set; }
        [RefersTo(TLCGenObjectTypeEnum.Input, nameof(InputTLCGenType))]
        [HasDefault(false)]
        public string RelatedInput2 { get; set; }
        public PrioIngreepInUitMeldingVoorwaardeInputTypeEnum RelatedInput2Type { get; set; }
        
        public bool KijkNaarWisselStand { get; set; }
        public bool AlleenIndienGeenInmelding { get; set; }
        public bool AlleenIndienRood { get; set; }

        public bool AntiJutterTijdToepassen { get; set; }
        public int AntiJutterTijd { get; set; }

        public bool OpvangStoring { get; set; }
        public PrioIngreepInUitMeldingModel MeldingBijstoring { get; set; }

        #endregion

        #region RIS

        public int RisStart { get; set; }
        public int RisEnd { get; set; }
        public bool RisMatchSg { get; set; }

        #endregion

        [Browsable(false)]
        public DetectorModel DummyKARMelding { get; set; }

        #endregion // Properties

        #region Constructor

        public PrioIngreepInUitMeldingModel()
        {
        }
        
        #endregion // Constructor
    }
}
