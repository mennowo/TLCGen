using System.ComponentModel;
using TLCGen.Helpers;

namespace TLCGen.Models.Enumerations
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum NooitAanUitEnum
    {
        [Description("Nooit")]
        Nooit,
        [Description("Aan")]
        SchAan,
        [Description("Uit")]
        SchUit,
    }
}
