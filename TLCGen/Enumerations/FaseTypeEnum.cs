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
    public enum FaseTypeEnum
    {
        [Description("Auto")]
        Auto,
        [Description("Voetganger")]
        Voetganger,
        [Description("Fiets")]
        Fiets,
        [Description("Tram")]
        Tram,
        [Description("Bus 1")]
        Bus
    }
}
