using System.ComponentModel;
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
