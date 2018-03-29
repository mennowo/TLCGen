using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.Extensions
{
    public static class TLCGenModelExtensions
    {
        public static bool HasOVIngreepDSI(this OVIngreepModel ov)
        {
            return ov.Meldingen.Any(x => x.Type == Models.Enumerations.OVIngreepMeldingTypeEnum.KAR ||
                                         x.Type == Models.Enumerations.OVIngreepMeldingTypeEnum.VECOM);
        }

        public static bool HasOVIngreepVecom(this OVIngreepModel ov)
        {
            return ov.Meldingen.Any(x => x.Type == Models.Enumerations.OVIngreepMeldingTypeEnum.VECOM);
        }

        public static bool HasOVIngreepKAR(this OVIngreepModel ov)
        {
            return ov.Meldingen.Any(x => x.Type == Models.Enumerations.OVIngreepMeldingTypeEnum.KAR);
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

        public static bool HasPTorHD(this ControllerModel c)
        {
            return c.OVData.OVIngrepen.Any() ||
                   c.OVData.HDIngrepen.Any();
        }

        public static string GetDefine(this DetectorModel d)
        {
            return CCOLGeneratorSettingsProvider.Default.GetPrefix("d") + d.Naam;
        }

        public static string GetDefine(this FaseCyclusModel fc)
        {
            return CCOLGeneratorSettingsProvider.Default.GetPrefix("fc") + fc.Naam;
        }

        public static string GetFaseFromDefine(this ConflictModel conf)
        {
            return CCOLGeneratorSettingsProvider.Default.GetPrefix("fc") + conf.FaseVan;
        }

        public static string GetFaseToDefine(this ConflictModel conf)
        {
            return CCOLGeneratorSettingsProvider.Default.GetPrefix("fc") + conf.FaseNaar;
        }

        public static string GetFaseCyclusDefine(this ModuleFaseCyclusModel mlfc)
        {
            return CCOLGeneratorSettingsProvider.Default.GetPrefix("fc") + mlfc.FaseCyclus;
        }

        public static string GetBitmapCoordinaatOutputDefine(this IOElementModel o, string name = null)
        {
            if (name != null)
            {
                return CCOLGeneratorSettingsProvider.Default.GetPrefix("us") + name;
            }
            else
            {
                return CCOLGeneratorSettingsProvider.Default.GetPrefix("us") + o.Naam;
            }
        }

        public static string GetBitmapCoordinaatInputDefine(this IOElementModel i)
        {
            return CCOLGeneratorSettingsProvider.Default.GetPrefix("us") + i.Naam;
        }
    }
}
