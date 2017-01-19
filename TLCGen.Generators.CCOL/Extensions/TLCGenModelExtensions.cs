using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.Extensions
{
    public static class TLCGenModelExtensions
    {
#warning todo: use settings here!

        public static string GetDefine(this DetectorModel d)
        {
            return "d" + d.Naam;
        }

        public static string GetDefine(this FaseCyclusModel fc)
        {
            return "fc" + fc.Naam;
        }

        public static string GetFaseFromDefine(this ConflictModel conf)
        {
            return "fc" + conf.FaseVan;
        }

        public static string GetFaseToDefine(this ConflictModel conf)
        {
            return "fc" + conf.FaseNaar;
        }

        public static string GetFaseCyclusDefine(this ModuleFaseCyclusModel mlfc)
        {
            return "fc" + mlfc.FaseCyclus;
        }

        public static string GetBitmapCoordinaatOutputDefine(this IOElementModel o, string name = null)
        {
            if (name != null)
            {
                return "us" + name;
            }
            else
            {
                return "us" + o.Naam;
            }
        }

        public static string GetBitmapCoordinaatInputDefine(this IOElementModel i)
        {
            return "us" + i.Naam;
        }
    }
}
