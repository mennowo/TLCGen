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

        [Browsable(false)]
        public PrioIngreepInUitMeldingTypeEnum InUit { get; set; }

        public PrioIngreepInUitMeldingVoorwaardeTypeEnum Type { get; set; }

        #region Reguliere IO (detectie, Vecom, SD via ingang, wissels)

        [Browsable(false)]
        [RefersTo(TLCGenObjectTypeEnum.Input, nameof(InputTLCGenType))]
        [HasDefault(false)]
        public string RelatedInput1 { get; set; }

        public PrioIngreepInUitMeldingVoorwaardeInputTypeEnum RelatedInput1Type { get; set; }

        [Browsable(false)]
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

        [Browsable(false)]
        public bool TweedeInput { get; set; }
        
        [Browsable(false)]
        [RefersTo(TLCGenObjectTypeEnum.Input, nameof(InputTLCGenType))]
        [HasDefault(false)]
        public string RelatedInput2 { get; set; }
        public PrioIngreepInUitMeldingVoorwaardeInputTypeEnum RelatedInput2Type { get; set; }
        
        [Browsable(false)]
        public bool KijkNaarWisselStand { get; set; }
        
        public bool AlleenIndienGeenInmelding { get; set; }
        
        public bool AlleenIndienRood { get; set; }

        public bool AntiJutterTijdToepassen { get; set; }
        
        public int AntiJutterTijd { get; set; }

        public bool CheckAltijdOpDsinBijVecom { get; set; }

        public bool OpvangStoring { get; set; }
        
        public PrioIngreepInUitMeldingModel MeldingBijstoring { get; set; }

        #endregion

        #region RIS

        public int RisStart { get; set; }
        public int RisEnd { get; set; }
        public int? RisEta { get; set; }
        public RISVehicleRole RisRole { get; set; }
        public RISVehicleSubrole RisSubrole { get; set; }
        public RISVehicleImportance RisImportance { get; set; }
        public bool RisPeloton { get; set; }
        public int RisPelotonGrenswaarde { get; set; }

        #endregion

        #region Fiets
        
        public bool FietsPrioriteitGebruikLus { get; set; }
        public int FietsPrioriteitBlok { get; set; }
        public int FietsPrioriteitAantalKeerPerCyclus { get; set; }
        public int FietsPrioriteitMinimumAantalVoertuigen { get; set; }
        public int FietsPrioriteitMinimumWachttijdVoorPrioriteit { get; set; }
        public bool FietsPrioriteitGebruikRIS { get; set; }
        public int FietsPrioriteitMinimumAantalVoertuigenRIS { get; set; }

        #endregion
        
        #region Peloton koppeling
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
