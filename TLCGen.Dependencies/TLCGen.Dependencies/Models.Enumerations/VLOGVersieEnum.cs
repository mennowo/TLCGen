using System.ComponentModel;
using TLCGen.Helpers;

namespace TLCGen.Models.Enumerations
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum VLOGVersieEnum
    {
        [Description("VLOG 3.0.x")]
        VLOG30x,
        [Description("VLOG 3.1.x")]
        VLOG31x,
        [Description("VLOG 3.3.x")]
        VLOG33x
    }
}
