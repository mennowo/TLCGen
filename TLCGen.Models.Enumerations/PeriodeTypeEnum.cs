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
        [Description("Rateltikkers")]
        Rateltikkers,
        [Description("Waarschuwingslichten")]
        WaarschuwingsLichten,
        [Description("Bellen")]
        Bellen,
        [Description("Overig")]
        Overig
    }
}
