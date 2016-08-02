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
    public enum CCOLVersieEnum
    {
        [Description("CCOL 8")]
        CCOL8
    }
}
