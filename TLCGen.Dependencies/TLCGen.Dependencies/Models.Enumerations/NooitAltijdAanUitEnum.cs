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
    public enum NooitAltijdAanUitEnum
    {
        [Description("Nooit")]
        Nooit,
        [Description("Altijd")]
        Altijd,
        [Description("Aan")]
        SchAan,
        [Description("Uit")]
        SchUit,
    }
}
