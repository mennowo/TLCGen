using System.ComponentModel;
using TLCGen.Helpers;

namespace TLCGen.Models.Enumerations
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum MeeVerlengenTypeEnum
    {
        [Description("ymmaxV1")]
        Default,
        [Description("ymmax_toV1")]
        To,
        [Description("ymmaxV1 || MK[fc] && ymmax_toV1")]
        MKTo,
        [Description("ymmax_vtg")]
        Voetganger,
        [Description("ymmax")]
        DefaultCCOL,
        [Description("ymmax_to")]
        ToCCOL,
        [Description("ymmax || MK[fc] && ymmax_to")]
        MKToCCOL,
        [Description("Maatgevend_Groen")]
        MaatgevendGroen
    }
}
