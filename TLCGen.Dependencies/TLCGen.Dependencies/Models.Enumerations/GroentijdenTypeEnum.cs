using System.ComponentModel;
using TLCGen.Helpers;

namespace TLCGen.Models.Enumerations
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum GroentijdenTypeEnum
    {
        [Description("Maxgroen")]
        MaxGroentijden,
        [Description("Verlenggroen")]
        VerlengGroentijden
    }
}
