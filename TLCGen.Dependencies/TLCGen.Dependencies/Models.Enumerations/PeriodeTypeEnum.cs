using System.ComponentModel;
using TLCGen.Helpers;

namespace TLCGen.Models.Enumerations
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum PeriodeTypeEnum
    {
        [Description("Groentijden")]
        Groentijden,
        [Description("Rateltikkers altijd")]
        RateltikkersAltijd,
        [Description("Rateltikkers op aanvraag")]
        RateltikkersAanvraag,
        [Description("Rateltikkers dimmen")]
        RateltikkersDimmen,
        [Description("Bellen actief")]
        BellenActief,
        [Description("Bellen dimmen")]
        BellenDimmen,
        [Description("Star regelen")]
        StarRegelen,
        [Description("Overig")]
        Overig
    }
}
