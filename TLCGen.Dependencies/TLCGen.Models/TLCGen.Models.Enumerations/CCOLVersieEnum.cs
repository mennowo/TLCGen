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
    public enum CCOLVersieEnum
    {
        [Description("8.0")]
        CCOL8,
        [Description("9.0")]
        CCOL9,
        [Description("9.5")]
        CCOL95
    }
}
