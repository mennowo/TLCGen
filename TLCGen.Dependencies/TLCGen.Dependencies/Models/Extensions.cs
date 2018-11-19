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

        public static bool HasOVIngreepVecomIO(this OVIngreepModel ov)
        {
            return ov.MeldingenData.Inmeldingen.Any(x => (x.Type == OVIngreepInUitMeldingVoorwaardeTypeEnum.VecomViaDetector)) ||
                   ov.MeldingenData.Uitmeldingen.Any(x => (x.Type == OVIngreepInUitMeldingVoorwaardeTypeEnum.VecomViaDetector));
        }

        public static bool HasOVIngreepDSI(this OVIngreepModel ov)
        {
            return ov.MeldingenData.Inmeldingen.Any(x => (x.Type == OVIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding || x.Type == OVIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector)) ||
                   ov.MeldingenData.Uitmeldingen.Any(x => (x.Type == OVIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding || x.Type == OVIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector));
        }

        public static bool HasOVIngreepVecom(this OVIngreepModel ov)
        {
            return ov.MeldingenData.Inmeldingen.Any(x => (x.Type == OVIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector)) ||
                   ov.MeldingenData.Uitmeldingen.Any(x => (x.Type == OVIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector));
        }

        public static bool HasOVIngreepKAR(this OVIngreepModel ov)
        {
            return ov.MeldingenData.Inmeldingen.Any(x => (x.Type == OVIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding)) ||
                   ov.MeldingenData.Uitmeldingen.Any(x => (x.Type == OVIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding));
        }

        public static bool HasOVIngreepWissel(this OVIngreepModel ov)
        {
            return ((ov.MeldingenData.Wissel1 &&
                     ((ov.MeldingenData.Wissel1Type == OVIngreepInUitDataWisselTypeEnum.Ingang && !string.IsNullOrWhiteSpace(ov.MeldingenData.Wissel1Input)) ||
                      (ov.MeldingenData.Wissel1Type == OVIngreepInUitDataWisselTypeEnum.Detector && !string.IsNullOrWhiteSpace(ov.MeldingenData.Wissel1Detector)))) ||
                    (ov.MeldingenData.Wissel2 &&
                     ((ov.MeldingenData.Wissel2Type == OVIngreepInUitDataWisselTypeEnum.Ingang && !string.IsNullOrWhiteSpace(ov.MeldingenData.Wissel2Input)) ||
                      (ov.MeldingenData.Wissel2Type == OVIngreepInUitDataWisselTypeEnum.Detector && !string.IsNullOrWhiteSpace(ov.MeldingenData.Wissel2Detector)))));
        }

        public static bool HasDSI(this ControllerModel c)
        {
            return c.OVData.OVIngrepen.Any(x => x.HasOVIngreepDSI()) ||
                   c.OVData.HDIngrepen.Any(x => x.KAR) ||
                   c.SelectieveDetectoren.Any(x => x.SdType == SelectieveDetectorTypeEnum.VECOM);
        }

        public static bool HasKAR(this ControllerModel c)
        {
            return c.OVData.OVIngrepen.Any(x => x.HasOVIngreepKAR()) ||
                   c.OVData.HDIngrepen.Any(x => x.KAR);
        }

        public static bool HasKAR(this OVDataModel ovdm)
        {
            return ovdm.OVIngrepen.Any(x => x.HasOVIngreepKAR()) ||
                   ovdm.HDIngrepen.Any(x => x.KAR);
        }

        public static bool HasPTorHD(this ControllerModel c)
        {
            return c.OVData.OVIngrepen.Any() ||
                   c.OVData.HDIngrepen.Any();
        }
    }
}
