using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    public static class Extensions
    {
        public static IEnumerable<DetectorModel> GetAllDetectors(this ControllerModel c)
        {
            return c.Fasen.SelectMany(x => x.Detectoren).Concat(c.Detectoren).Concat(c.SelectieveDetectoren);
        }

        public static IEnumerable<DetectorModel> GetAllDetectors(this ControllerModel c, Func<DetectorModel, bool> predicate)
        {
            return c.Fasen.SelectMany(x => x.Detectoren).Concat(c.Detectoren).Concat(c.SelectieveDetectoren).Where(predicate);
        }

        public static FaseCyclusModel GetFaseCyclus(this ControllerModel c, string naam)
        {
            return c.Fasen.FirstOrDefault(x => x.Naam == naam);
        }

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
    }
}
