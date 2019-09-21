using System.ComponentModel;
using TLCGen.Helpers;

namespace TLCGen.Models.Enumerations
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
