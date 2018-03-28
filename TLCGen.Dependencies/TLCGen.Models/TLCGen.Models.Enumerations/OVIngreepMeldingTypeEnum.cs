using System.ComponentModel;
using TLCGen.Helpers;

namespace TLCGen.Models.Enumerations
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum OVIngreepMeldingTypeEnum
    {
        [Description("VECOM")]
        VECOM,
        [Description("VECOM middels ingangen")]
        VECOM_io,
        [Description("KAR")]
        KAR,
        [Description("Opticom")]
        Opticom,
        [Description("Verlos detector")]
        VerlosDetector,
        [Description("Wissel stroomkring detector")]
        WisselStroomKringDetector,
        [Description("Wissel detector")]
        WisselDetector,
        [Description("Lussenpaar in")]
        MassaPaarIn,
        [Description("Lussenpaar uit")]
        MassaPaarUit,
    }
}
