using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    public static class ControllerModelExtensions
    {
        #region Detectors

        public static IEnumerable<DetectorModel> GetAllDetectors(this ControllerModel c, bool noDummies = false)
        {
            return noDummies 
                ? c.Fasen.SelectMany(x => x.Detectoren.Where(x2 => !x2.Dummy)).Concat(c.Detectoren.Where(x => !x.Dummy)).Concat(c.SelectieveDetectoren.Where(x => !x.Dummy))
                : c.Fasen.SelectMany(x => x.Detectoren).Concat(c.Detectoren).Concat(c.SelectieveDetectoren);
        }
        
        public static IEnumerable<DetectorModel> GetAllDetectors(this ControllerModel c, Func<DetectorModel, bool> predicate)
        {
            return c.Fasen.SelectMany(x => x.Detectoren).Concat(c.Detectoren).Concat(c.SelectieveDetectoren).Where(predicate);
        }

        public static IEnumerable<DetectorModel> GetAllRegularDetectors(this ControllerModel c)
        {
            return c.Fasen.SelectMany(x => x.Detectoren).Concat(c.Detectoren);
        }
        
        #endregion // Detectors

        #region SignalGroups
        
        public static FaseCyclusModel GetFaseCyclus(this ControllerModel c, string naam)
        {
            return c.Fasen.FirstOrDefault(x => x.Naam == naam);
        }

        /// <summary>
        /// Appends a string to a StringBuilder per SignalGroup in a ControllerModel
        /// <remarks>Use placeholder &lt;FC&gt; to mark where _fcpf + fc.Naam should be placed</remarks>
        /// </summary>
        /// <param name="c">The Controller</param>
        /// <param name="fcPf">FC prefix</param>
        /// <param name="s">The string to be added</param>
        public static void AppendPerFase(this StringBuilder sb, ControllerModel c, string fcPf, string s)
        {
            foreach (var fc in c.Fasen)
            {
                sb.AppendLine(s.Replace("<FC>", fcPf + fc.Naam));
            }
        }
        
        #endregion // SignalGroups

        #region Synchronisations

        public static IEnumerable<IInterSignaalGroepElement> GetAllSynchronisations(this ControllerModel c)
        {
            return c.InterSignaalGroep.Gelijkstarten
                .Cast<IInterSignaalGroepElement>()
                .Concat(c.InterSignaalGroep.Voorstarten)
                .Concat(c.InterSignaalGroep.LateReleases)
                .Concat(c.InterSignaalGroep.Nalopen);
        }

        public static IEnumerable<NaloopModel> GetVoetgangersNalopen(this ControllerModel c)
        {
            return c.InterSignaalGroep.Nalopen.Where(x =>
                c.Fasen.Any(x2 => x2.Naam == x.FaseVan && x2.Type == FaseTypeEnum.Voetganger) &&
                c.Fasen.Any(x2 => x2.Naam == x.FaseNaar && x2.Type == FaseTypeEnum.Voetganger));
        }
        
        public static IEnumerable<GelijkstartModel> GetGelijkstarten(this ControllerModel c, string fc)
        {
            return c.InterSignaalGroep.Gelijkstarten.Where(x => x.FaseVan == fc || x.FaseNaar == fc);
        }
        
        public static IEnumerable<VoorstartModel> GetVoorstarten(this ControllerModel c, string fcFrom)
        {
            return c.InterSignaalGroep.Voorstarten.Where(x => x.FaseVan == fcFrom);
        }
        
        public static IEnumerable<VoorstartModel> GetVoorstartenNaar(this ControllerModel c, string fcTo)
        {
            return c.InterSignaalGroep.Voorstarten.Where(x => x.FaseNaar == fcTo);
        }
        
        public static IEnumerable<LateReleaseModel> GetLateReleases(this ControllerModel c, string fcFrom)
        {
            return c.InterSignaalGroep.LateReleases.Where(x => x.FaseVan == fcFrom);
        }
        
        public static IEnumerable<LateReleaseModel> GetLateReleasesNaar(this ControllerModel c, string fcTo)
        {
            return c.InterSignaalGroep.LateReleases.Where(x => x.FaseNaar == fcTo);
        }

        #endregion // Synchronisations
        
        #region Public Transport

        public static bool HasOVIngreepVecomIO(this PrioIngreepModel ov)
        {
            return ov.MeldingenData.Inmeldingen.Any(x => x.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.VecomViaDetector) ||
                   ov.MeldingenData.Uitmeldingen.Any(x => x.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.VecomViaDetector);
        }

        public static bool HasOVIngreepDSI(this PrioIngreepModel ov)
        {
            return ov.MeldingenData.Inmeldingen.Any(x => x.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding || x.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector) ||
                   ov.MeldingenData.Uitmeldingen.Any(x => x.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding || x.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector);
        }

        public static bool HasOVIngreepVecom(this PrioIngreepModel ov)
        {
            return ov.MeldingenData.Inmeldingen.Any(x => x.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector) ||
                   ov.MeldingenData.Uitmeldingen.Any(x => x.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector);
        }

        public static bool HasPrioIngreepKAR(this PrioIngreepModel ov)
        {
            return ov.MeldingenData.Inmeldingen.Any(x => x.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding) ||
                   ov.MeldingenData.Uitmeldingen.Any(x => x.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding);
        }

        public static IEnumerable<DetectorModel> GetDummyInDetectors(this PrioIngreepModel ov)
        {
            return ov.MeldingenData.Inmeldingen.Where(x => x.DummyKARMelding != null).Select(x => x.DummyKARMelding);
        }

        public static IEnumerable<DetectorModel> GetDummyUitDetectors(this PrioIngreepModel ov)
        {
            return ov.MeldingenData.Uitmeldingen.Where(x => x.DummyKARMelding != null).Select(x => x.DummyKARMelding);
        }

        public static bool HasOVIngreepWissel(this PrioIngreepModel ov)
        {
            return ov.MeldingenData.Wissel1 &&
                   (ov.MeldingenData.Wissel1Type == PrioIngreepInUitDataWisselTypeEnum.Ingang && !string.IsNullOrWhiteSpace(ov.MeldingenData.Wissel1Input) ||
                    ov.MeldingenData.Wissel1Type == PrioIngreepInUitDataWisselTypeEnum.Detector && !string.IsNullOrWhiteSpace(ov.MeldingenData.Wissel1Detector)) ||
                   ov.MeldingenData.Wissel2 &&
                   (ov.MeldingenData.Wissel2Type == PrioIngreepInUitDataWisselTypeEnum.Ingang && !string.IsNullOrWhiteSpace(ov.MeldingenData.Wissel2Input) ||
                    ov.MeldingenData.Wissel2Type == PrioIngreepInUitDataWisselTypeEnum.Detector && !string.IsNullOrWhiteSpace(ov.MeldingenData.Wissel2Detector));
        }

        public static bool HasDSI(this ControllerModel c)
        {
            return c.PrioData.PrioIngrepen.Any(x => x.HasOVIngreepDSI()) ||
                   c.PrioData.HDIngrepen.Any(x => x.KAR) ||
                   c.SelectieveDetectoren.Any(x => x.SdType == SelectieveDetectorTypeEnum.VECOM);
        }

        public static bool HasKAR(this ControllerModel c)
        {
            return c.PrioData.PrioIngrepen.Any(x => x.HasPrioIngreepKAR()) ||
                   c.PrioData.HDIngrepen.Any(x => x.KAR);
        }

        public static bool HasVecom(this ControllerModel c)
        {
            return c.SelectieveDetectoren.Any(x => x.SdType == SelectieveDetectorTypeEnum.VECOM) &&
                   c.PrioData.PrioIngrepen.Any(x => x.HasOVIngreepVecom());
        }

        public static bool HasDeelConflict(this ControllerModel c)
        {
            return c.InterSignaalGroep.Gelijkstarten.Any() ||
                   c.InterSignaalGroep.Voorstarten.Any() ||
                   c.InterSignaalGroep.LateReleases.Any();
        }
        
        public static bool HasVecomIO(this ControllerModel c)
        {
            return c.GetAllDetectors(x => x.Type == DetectorTypeEnum.VecomDetector).Any() &&
                   c.PrioData.PrioIngrepen.Any(x => x.HasOVIngreepVecomIO());
        }

        public static bool HasKAR(this PrioriteitDataModel ovdm)
        {
            return ovdm.PrioIngrepen.Any(x => x.HasPrioIngreepKAR()) ||
                   ovdm.HDIngrepen.Any(x => x.KAR);
        }

        public static bool HasPT(this ControllerModel c)
        {
            return c.PrioData.PrioIngrepen.Any();
        }

        public static bool HasHD(this ControllerModel c)
        {
            return c.PrioData.HDIngrepen.Any();
        }
        
        public static bool HasPrioRis(this ControllerModel c)
        {
            return c.PrioData.HDIngrepen.Any(x => x.RIS) ||
                   c.PrioData.PrioIngrepen.Any(x => x.MeldingenData.Inmeldingen.Any(x2 => x2.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde) ||
                                                    x.MeldingenData.Uitmeldingen.Any(x2 => x2.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde));
        }

        public static IEnumerable<PrioIngreepModel> GetPrioIngrepen(this ControllerModel c, string faseCyclus)
        {
            return c.PrioData.PrioIngrepen.Where(x => x.FaseCyclus == faseCyclus);
        }

        public static bool HasHDKAR(this ControllerModel c)
        {
            return c.PrioData.HDIngrepen.Any(x => x.KAR);
        }

        public static bool HasHDOpticom(this ControllerModel c)
        {
            return c.PrioData.HDIngrepen.Any(x => x.Opticom && !string.IsNullOrWhiteSpace(x.OpticomRelatedInput));
        }

        public static bool HasPTorHD(this ControllerModel c)
        {
            return c.PrioData.PrioIngrepen.Any() ||
                   c.PrioData.HDIngrepen.Any();
        }

        #endregion // Public Transport
    }
}
