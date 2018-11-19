using System.ComponentModel;
using TLCGen.Helpers;

namespace TLCGen.Models.Enumerations
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum HardMeevelengenTypeEnum
    {
        [Description("Groen")]
        Groen,
        [Description("CV")]
        CyclischVerlengGroen,
        [Description("CV of groen")]
        CyclischVerlengGroenEnGroen,
    }
}
