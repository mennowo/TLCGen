using System.Collections.Generic;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    /// <summary>
    /// The melding types (in- and uitmeldingen) that can be chosen for a priority ingreep.
    /// </summary>
    public static class PrioMeldingTypes
    {
        /// <summary>
        /// The type of a melding bij storing (fallback melding, used when the detector of the melding fails).
        /// </summary>
        public const PrioIngreepInUitMeldingVoorwaardeTypeEnum MeldingBijStoringType = PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector;

        /// <summary>
        /// Opvang storing (a fallback melding on detector failure) is only possible for melding types with a detector
        /// whose failure can be detected: Detector and Vecom (via detector).
        /// </summary>
        public static bool SupportsOpvangStoring(PrioIngreepInUitMeldingVoorwaardeTypeEnum type) =>
            type is PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector or PrioIngreepInUitMeldingVoorwaardeTypeEnum.VecomViaDetector;

        /// <summary>
        /// Whether a melding has a valid opvang storing: enabled, a supported type and a fallback melding of the
        /// right type. Anything else (eg. left over in older files) is ignored.
        /// </summary>
        public static bool HasValidOpvangStoring(PrioIngreepInUitMeldingModel melding) =>
            melding.OpvangStoring && melding.MeldingBijstoring != null &&
            SupportsOpvangStoring(melding.Type) &&
            melding.MeldingBijstoring.Type == MeldingBijStoringType;

        /// <summary>
        /// The melding types allowed for a vehicle type, in the order they are offered in the UI. RIS meldingen
        /// need CCOL 11 or higher, except for hulpdiensten.
        /// </summary>
        public static IReadOnlyList<PrioIngreepInUitMeldingVoorwaardeTypeEnum> GetAllowed(PrioIngreepVoertuigTypeEnum vehicleType, CCOLVersieEnum ccolVersie)
        {
            var ris = ccolVersie >= CCOLVersieEnum.CCOL110;
            var types = new List<PrioIngreepInUitMeldingVoorwaardeTypeEnum>();
            switch (vehicleType)
            {
                case PrioIngreepVoertuigTypeEnum.Tram:
                case PrioIngreepVoertuigTypeEnum.Bus:
                    types.Add(PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector);
                    types.Add(PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding);
                    types.Add(PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector);
                    types.Add(PrioIngreepInUitMeldingVoorwaardeTypeEnum.VecomViaDetector);
                    if (ris) types.Add(PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde);
                    types.Add(PrioIngreepInUitMeldingVoorwaardeTypeEnum.Ingang);
                    break;
                case PrioIngreepVoertuigTypeEnum.Fiets:
                    types.Add(PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector);
                    types.Add(PrioIngreepInUitMeldingVoorwaardeTypeEnum.FietsMassaPeloton);
                    if (ris) types.Add(PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde);
                    break;
                case PrioIngreepVoertuigTypeEnum.Vrachtwagen:
                    types.Add(PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector);
                    if (ris) types.Add(PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde);
                    break;
                case PrioIngreepVoertuigTypeEnum.Auto:
                    types.Add(PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector);
                    types.Add(PrioIngreepInUitMeldingVoorwaardeTypeEnum.AutoMassaPeloton);
                    if (ris) types.Add(PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde);
                    break;
                case PrioIngreepVoertuigTypeEnum.Hulpdienst:
                    types.Add(PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding);
                    types.Add(PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde);
                    types.Add(PrioIngreepInUitMeldingVoorwaardeTypeEnum.Opticom);
                    types.Add(PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector);
                    types.Add(PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector);
                    types.Add(PrioIngreepInUitMeldingVoorwaardeTypeEnum.VecomViaDetector);
                    break;
                case PrioIngreepVoertuigTypeEnum.NG:
                    types.Add(PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector);
                    types.Add(PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding);
                    types.Add(PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector);
                    types.Add(PrioIngreepInUitMeldingVoorwaardeTypeEnum.VecomViaDetector);
                    types.Add(PrioIngreepInUitMeldingVoorwaardeTypeEnum.FietsMassaPeloton);
                    types.Add(PrioIngreepInUitMeldingVoorwaardeTypeEnum.AutoMassaPeloton);
                    if (ris) types.Add(PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde);
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(vehicleType));
            }
            return types;
        }
    }
}
