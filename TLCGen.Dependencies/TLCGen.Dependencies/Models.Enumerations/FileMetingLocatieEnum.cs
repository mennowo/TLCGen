using System.ComponentModel;
using TLCGen.Helpers;

namespace TLCGen.Models.Enumerations
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum FileMetingLocatieEnum
    {
        [Description("Na stopstreep")]
        NaStopstreep,
        [Description("Voor stopstreep")]
        VoorStopstreep
    }
}
