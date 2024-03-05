using System.ComponentModel;
using TLCGen.Helpers;

namespace TLCGen.Models.Enumerations
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum MeeVerlengenTypeEnum
    {
        [Description("ym_maxV1")]
        Default = 0,
        [Description("ym_max_toV1")]
        To = 1,
        [Description("ym_maxV1 || MK[fc] && ym_max_toV1")]
        MKTo = 2,
        [Description("ym_max_vtgV1")]
        Voetganger = 3,
        [Description("ym_max")]
        DefaultCCOL = 4,
        [Description("ym_max_to")]
        ToCCOL = 5,
        [Description("ym_max || MK[fc] && ym_max_to")]
        MKToCCOL = 6,
        [Description("Maatgevend_Groen")]
        MaatgevendGroen = 7,
        [Description("ym_maxV2")]
        Default2 = 8, 
        [Description("ym_max_toV2")]
        To2 = 9,
        [Description("ym_maxV2 || MK[fc] && ym_max_toV2")]
        MKTo2,
        [Description("ym_max_vtgV2")]
        Voetganger2,
    }
}
