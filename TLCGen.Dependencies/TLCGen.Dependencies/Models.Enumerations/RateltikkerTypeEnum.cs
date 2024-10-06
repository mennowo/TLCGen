using System.ComponentModel;
using TLCGen.Helpers;

namespace TLCGen.Models.Enumerations
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum RateltikkerTypeEnum
    {
        Geen,
        [Description("TEC / Hoeflake")]
        Hoeflake,
        [Description("Hoeflake (bewaakt)")]
        HoeflakeBewaakt,
        Accross
    }
}
