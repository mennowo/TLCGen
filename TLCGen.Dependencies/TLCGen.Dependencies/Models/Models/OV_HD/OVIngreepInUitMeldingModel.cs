using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class OVIngreepInUitMeldingModel
    {
        #region Properties

        public OVIngreepInUitMeldingTypeEnum InUit { get; set; }
        public OVIngreepInUitMeldingVoorwaardeTypeEnum Type { get; set; }

        [RefersTo(TLCGenObjectTypeEnum.Input)]
        [HasDefault(false)]
        public string RelatedInput1 { get; set; }
        public OVIngreepInUitMeldingVoorwaardeInputTypeEnum RelatedInput1Type { get; set; }

        public bool TweedeInput { get; set; }
        [RefersTo(TLCGenObjectTypeEnum.Input)]
        [HasDefault(false)]
        public string RelatedInput2 { get; set; }
        public OVIngreepInUitMeldingVoorwaardeInputTypeEnum RelatedInput2Type { get; set; }

        public bool KijkNaarWisselStand { get; set; }
        public bool AlleenIndienGeenInmelding { get; set; }

        public bool AntiJutterTijdToepassen { get; set; }
        public int AntiJutterTijd { get; set; }

        public bool OpvangStoring { get; set; }
        public OVIngreepInUitMeldingModel MeldingBijstoring { get; set; }
        
        #endregion // Properties
        
        #region Constructor

        public OVIngreepInUitMeldingModel()
        {
        }
        
        #endregion // Constructor
    }
}
