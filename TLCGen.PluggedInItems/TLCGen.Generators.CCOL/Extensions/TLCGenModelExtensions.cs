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
