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
    public enum KWCTypeEnum
    {
        [Description("Geen")]
        Geen,
        [Description("Ko Hartog")]
        KoHartog,
        [Description("Swarco")]
        Swarco,
        [Description("Dynniq")]
        Dynniq,
        [Description("Vialis")]
        Vialis
    }
}
