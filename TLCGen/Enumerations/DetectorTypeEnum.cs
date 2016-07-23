using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Helpers;

namespace TLCGen.Enumerations
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum DetectorType
    {
        [Description("Koplus")]
        Kop,
        [Description("Lange lus")]
        Lang,
        [Description("Verweg lus")]
        Verweg,
        [Description("File lus")]
        File,
        [Description("Drukknop")]
        Knop,
        [Description("Radar")]
        Radar
    }
}
