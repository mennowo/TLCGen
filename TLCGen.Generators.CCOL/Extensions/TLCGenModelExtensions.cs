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
            return "d" + fc.Naam;
        }
    }
}
