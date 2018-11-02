using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Settings.Utilities
{
    public static class FaseCyclusUtilities
    {
        public static FaseTypeEnum GetFaseTypeFromNaam(string naam)
        {
            int myfase = -1;
            if (Int32.TryParse(naam, out myfase))
            {
                myfase %= 100;
                if (myfase < 1) return FaseTypeEnum.Fiets;
                else if (myfase < 20) return FaseTypeEnum.Auto;
                else if (myfase < 30) return FaseTypeEnum.Fiets;
                else if (myfase < 40) return FaseTypeEnum.Voetganger;
                else if (myfase < 60) return FaseTypeEnum.OV;
                else if (myfase < 80) return FaseTypeEnum.Auto;
                else if (myfase < 90) return FaseTypeEnum.Fiets;
                else if (myfase < 100) return FaseTypeEnum.Voetganger;
            }
            return FaseTypeEnum.Fiets;
        }

        public static int? GetFaseDefaultGroenTijd(string naam)
        {
            return GetFaseDefaultGroenTijd(GetFaseTypeFromNaam(naam));
        }

        public static int? GetFaseDefaultGroenTijd(FaseTypeEnum type)
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
