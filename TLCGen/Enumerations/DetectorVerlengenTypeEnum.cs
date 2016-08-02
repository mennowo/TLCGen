using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Helpers;

namespace TLCGen.Enumerations
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum DetectorVerlengenTypeEnum
    {
        [Description("Geen")]
        Geen,
        [Description("Uitgeschakeld")]
        Uit,
        [Description("Kopmax")]
        Kopmax,
        [Description("2e kriterium")]
        MK2
    }
}
