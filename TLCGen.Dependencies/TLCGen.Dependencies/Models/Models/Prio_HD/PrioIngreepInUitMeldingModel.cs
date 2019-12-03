using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class PrioIngreepInUitMeldingModel
    {
        #region Properties

        public PrioIngreepInUitMeldingTypeEnum InUit { get; set; }
        public PrioIngreepInUitMeldingVoorwaardeTypeEnum Type { get; set; }

        [RefersTo(TLCGenObjectTypeEnum.Input)]
        [HasDefault(false)]
        public string RelatedInput1 { get; set; }
        public PrioIngreepInUitMeldingVoorwaardeInputTypeEnum RelatedInput1Type { get; set; }

        public bool TweedeInput { get; set; }
        [RefersTo(TLCGenObjectTypeEnum.Input)]
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
        
        #endregion // Properties
        
        #region Constructor

        public PrioIngreepInUitMeldingModel()
        {
        }
        
        #endregion // Constructor
    }
}
