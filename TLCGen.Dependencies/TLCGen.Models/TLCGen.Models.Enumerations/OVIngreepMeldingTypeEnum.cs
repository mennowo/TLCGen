using System.ComponentModel;
using TLCGen.Helpers;

namespace TLCGen.Models.Enumerations
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum OVIngreepMeldingTypeEnum
    {
        [Description("VECOM")]
        VECOM,
        [Description("KAR")]
        KAR,
        [Description("Verlos detector")]
        VerlosDetector,
        [Description("Wissel stroomkring detector")]
        WisselStroomKringDetector,
        [Description("Wissel detector")]
        WisselDetector
    }
}
