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
        [Description("Fiets")]
        Fiets,
        [Description("Voetganger")]
        Voetganger,
        [Description("OV")]
        OV
    }
}
