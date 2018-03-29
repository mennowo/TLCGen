using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    public static class Extensions
    {
        public static bool HasOVIngreepDSI(this OVIngreepModel ov)
        {
            return ov.Meldingen.Any(x => (x.Inmelding || x.Uitmelding) && 
                                         (x.Type == Enumerations.OVIngreepMeldingTypeEnum.KAR ||
                                          x.Type == Enumerations.OVIngreepMeldingTypeEnum.VECOM));
        }

        public static bool HasOVIngreepVecom(this OVIngreepModel ov)
        {
            return ov.Meldingen.Any(x => (x.Inmelding || x.Uitmelding) &&
                                         x.Type == Enumerations.OVIngreepMeldingTypeEnum.VECOM);
        }

        public static bool HasOVIngreepKAR(this OVIngreepModel ov)
        {
            return ov.Meldingen.Any(x => (x.Inmelding || x.Uitmelding) &&
                                         x.Type == Enumerations.OVIngreepMeldingTypeEnum.KAR);
        }

        public static bool HasDSI(this ControllerModel c)
        {
            return c.OVData.OVIngrepen.Any(x => x.HasOVIngreepDSI()) ||
                   c.OVData.HDIngrepen.Any(x => x.KAR);
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
