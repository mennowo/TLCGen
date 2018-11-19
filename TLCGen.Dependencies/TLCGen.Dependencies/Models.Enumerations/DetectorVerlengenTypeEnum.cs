using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Helpers;

namespace TLCGen.Models.Enumerations
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum DetectorVerlengenTypeEnum
    {
        [Description("Geen")]
        Geen = 4,
        [Description("Uitgeschakeld")]
        Uit = 0,
        [Description("Kopmax")]
        Kopmax = 1,
        [Description("1e kriterium")]
        MK1 = 2,
        [Description("2e kriterium")]
        MK2 = 3
    }
}
