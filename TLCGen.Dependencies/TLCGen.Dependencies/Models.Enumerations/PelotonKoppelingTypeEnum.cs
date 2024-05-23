using System.ComponentModel;
using TLCGen.Helpers;

namespace TLCGen.Models.Enumerations
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum PelotonKoppelingTypeEnum
    {
        [Description("Type 1")]
        DenHaag,
        [Description("Type 2")]
        RHDHV
    }
}
