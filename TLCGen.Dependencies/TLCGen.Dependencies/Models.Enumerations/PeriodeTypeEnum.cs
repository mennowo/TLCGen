using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        [Description("Overig")]
        Overig
    }
}
