using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TLCGen.DataAccess;
using TLCGen.Models.Enumerations;

namespace TLCGen.Utilities
{
    public static class FaseCyclusUtilities
    {
        public static FaseTypeEnum GetFaseTypeFromDefine(string define)
        {
            string faseswitch = Regex.Replace(define, SettingsProvider.GetFaseCyclusDefinePrefix(), "");
            if (faseswitch.Length >= 3)
                faseswitch = faseswitch.Substring(1);
            int myfase = -1;
            if (Int32.TryParse(faseswitch, out myfase))
            {
                if (myfase < 1) return FaseTypeEnum.Fiets;
                else if (myfase <  20) return FaseTypeEnum.Auto;
                else if (myfase <  30) return FaseTypeEnum.Fiets;
                else if (myfase <  40) return FaseTypeEnum.Voetganger;
                else if (myfase <  60) return FaseTypeEnum.OV;
                else if (myfase <  80) return FaseTypeEnum.Auto;
                else if (myfase <  90) return FaseTypeEnum.Fiets;
                else if (myfase < 100) return FaseTypeEnum.Voetganger;
            }
            return FaseTypeEnum.Fiets;
        }

        public static int? GetFaseDefaultMaxGroenTijd(string define)
        {
            return GetFaseDefaultMaxGroenTijd(GetFaseTypeFromDefine(define));
        }

        public static int? GetFaseDefaultMaxGroenTijd(FaseTypeEnum type)
        {
            switch (type)
            {
                case FaseTypeEnum.Auto:
                    return 300;
                case FaseTypeEnum.Voetganger:
                    return null;
                case FaseTypeEnum.OV:
                    return null;
                default:
                case FaseTypeEnum.Fiets:
                    return 150;
            }
        }
    }
}
