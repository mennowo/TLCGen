using System.Collections.Generic;
using System.Linq;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    /// <summary>
    /// Single owner of the invariants of a priority ingreep that depend on its vehicle type, its name or the types
    /// of its meldingen: allowed melding types, RIS roles and dummy KAR detectors. View-models call these instead
    /// of changing the related data themselves, so the rules can be tested without WPF.
    /// Only the top level meldingen are handled; meldingen bij storing (MeldingBijstoring) are left as they are.
    /// </summary>
    public static class PrioIngreepModelSync
    {
        /// <summary>
        /// Changes the vehicle type of an ingreep: meldingen of a type that is no longer allowed get the first allowed
        /// type, every melding gets the RIS roles of the new vehicle type and the dummy KAR detectors are updated.
        /// Default values for the vehicle type (DefaultsProvider) are NOT applied here.
        /// </summary>
        public static void ApplyVehicleTypeChange(PrioIngreepModel prio, PrioIngreepVoertuigTypeEnum vehicleType, CCOLVersieEnum ccolVersie)
        {
            PrioTrace.Write($"{prio.FaseCyclus}{prio.Naam}: vehicle type {prio.Type} -> {vehicleType}");
            prio.Type = vehicleType;
            ApplyAllowedMeldingTypes(prio, ccolVersie);
            foreach (var melding in GetMeldingen(prio))
            {
                SetRisRoles(melding, vehicleType);
            }
            prio.EnsureDummyKARDetectors();
        }

        /// <summary>
        /// Gives meldingen of a type that is not allowed for the vehicle type and CCOL version the first allowed type,
        /// drops their melding bij storing if the new type has no opvang storing, and updates the dummy KAR
        /// detectors. Returns the meldingen whose type was changed.
        /// </summary>
        public static IReadOnlyList<PrioIngreepInUitMeldingModel> ApplyAllowedMeldingTypes(PrioIngreepModel prio, CCOLVersieEnum ccolVersie)
        {
            var allowed = PrioMeldingTypes.GetAllowed(prio.Type, ccolVersie);
            var changed = new List<PrioIngreepInUitMeldingModel>();
            if (allowed.Count == 0) return changed;
            foreach (var melding in GetMeldingen(prio).Where(x => !allowed.Contains(x.Type)))
            {
                PrioTrace.Write($"{prio.FaseCyclus}{prio.Naam}: {melding.InUit} {melding.Type} not allowed, -> {allowed[0]}");
                melding.Type = allowed[0];
                if (!PrioMeldingTypes.SupportsOpvangStoring(melding.Type))
                {
                    melding.OpvangStoring = false;
                    melding.MeldingBijstoring = null;
                }
                changed.Add(melding);
            }
            prio.EnsureDummyKARDetectors();
            return changed;
        }

        /// <summary>
        /// Sets the RIS role and subrole of a melding to those of the vehicle type. NG leaves them as they are.
        /// </summary>
        public static void SetRisRoles(PrioIngreepInUitMeldingModel melding, PrioIngreepVoertuigTypeEnum vehicleType)
        {
            switch (vehicleType)
            {
                case PrioIngreepVoertuigTypeEnum.Tram:
                    melding.RisRole = RISVehicleRole.PUBLICTRANSPORT;
                    melding.RisSubrole = RISVehicleSubrole.TRAM;
                    break;
                case PrioIngreepVoertuigTypeEnum.Bus:
                    melding.RisRole = RISVehicleRole.PUBLICTRANSPORT;
                    melding.RisSubrole = RISVehicleSubrole.BUS;
                    break;
                case PrioIngreepVoertuigTypeEnum.Fiets:
                    melding.RisRole = RISVehicleRole.DEFAULT;
                    melding.RisSubrole = RISVehicleSubrole.UNKNOWN;
                    break;
                case PrioIngreepVoertuigTypeEnum.Vrachtwagen:
                    melding.RisRole = RISVehicleRole.COMMERCIAL | RISVehicleRole.SPECIALTRANSPORT;
                    melding.RisSubrole = RISVehicleSubrole.UNKNOWN;
                    break;
                case PrioIngreepVoertuigTypeEnum.Auto:
                    melding.RisRole = RISVehicleRole.DEFAULT;
                    melding.RisSubrole = RISVehicleSubrole.UNKNOWN;
                    break;
                case PrioIngreepVoertuigTypeEnum.Hulpdienst:
                    melding.RisRole = RISVehicleRole.EMERGENCY;
                    melding.RisSubrole = RISVehicleSubrole.EMERGENCY;
                    break;
                case PrioIngreepVoertuigTypeEnum.NG:
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(vehicleType));
            }
        }

        /// <summary>
        /// Renames an ingreep and its dummy KAR detectors. Validation of the name is up to the caller.
        /// </summary>
        public static void RenameIngreep(PrioIngreepModel prio, string naam)
        {
            PrioTrace.Write($"{prio.FaseCyclus}{prio.Naam}: renamed to {prio.FaseCyclus}{naam}");
            prio.Naam = naam;
            prio.EnsureDummyKARDetectors();
            prio.RenameDummyKARDetectors();
        }

        public static string GetDummyKARDetectorName(this PrioIngreepModel prio, PrioIngreepInUitMeldingTypeEnum inUit)
        {
            return $"dummykar{(inUit == PrioIngreepInUitMeldingTypeEnum.Inmelding ? "in" : "uit")}{prio.FaseCyclus}{prio.Naam}";
        }

        /// <summary>
        /// Single owner of the dummy KAR detectors of a priority ingreep, regardless of vehicle type:
        /// every (top level) KAR in- or uitmelding has a dummy detector, other meldingen have none.
        /// Existing dummies are kept as they are (name and simulation data).
        /// </summary>
        public static void EnsureDummyKARDetectors(this PrioIngreepModel prio)
        {
            if (prio.MeldingenData == null) return;
            foreach (var melding in GetMeldingen(prio))
            {
                if (melding.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding)
                {
                    if (melding.DummyKARMelding != null) PrioTrace.Write($"{prio.FaseCyclus}{prio.Naam}: dummy {melding.DummyKARMelding.Naam} removed ({melding.InUit} is {melding.Type})");
                    melding.DummyKARMelding = null;
                    continue;
                }
                if (melding.DummyKARMelding != null) continue;
                melding.DummyKARMelding = new DetectorModel
                {
                    Dummy = true,
                    Naam = prio.GetDummyKARDetectorName(melding.InUit)
                };
                PrioTrace.Write($"{prio.FaseCyclus}{prio.Naam}: dummy {melding.DummyKARMelding.Naam} added");
            }
        }

        /// <summary>
        /// Renames all dummy KAR detectors of a priority ingreep to the canonical name, eg. after renaming the ingreep.
        /// </summary>
        public static void RenameDummyKARDetectors(this PrioIngreepModel prio)
        {
            if (prio.MeldingenData == null) return;
            foreach (var melding in GetMeldingen(prio).Where(x => x.DummyKARMelding != null))
            {
                melding.DummyKARMelding.Naam = prio.GetDummyKARDetectorName(melding.InUit);
            }
        }

        private static IEnumerable<PrioIngreepInUitMeldingModel> GetMeldingen(PrioIngreepModel prio)
        {
            return prio.MeldingenData == null
                ? Enumerable.Empty<PrioIngreepInUitMeldingModel>()
                : prio.MeldingenData.Inmeldingen.Concat(prio.MeldingenData.Uitmeldingen);
        }
    }
}
