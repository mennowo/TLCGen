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
