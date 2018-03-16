using System.ComponentModel;
using TLCGen.Helpers;

namespace TLCGen.Models.Enumerations
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum OVIngreepMeldingTypeEnum
    {
        VECOM,
        KAR,
        VerlosDetector,
        WisselStandDetector,
        WisselStroomKringDetector,
        WisselDetector
    }
}
