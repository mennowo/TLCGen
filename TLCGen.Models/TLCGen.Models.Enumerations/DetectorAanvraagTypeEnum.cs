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
    public enum DetectorAanvraagTypeEnum
    {
        [Description("Geen")]
        Geen,
        [Description("Uitgeschakeld")]
        Uit,
        [Description("R en niet TRG")]
        RnietTRG,
        [Description("Rood")]
        Rood,
        [Description("Rood of geel")]
        RoodGeel
    }
}
