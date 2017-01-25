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
    public enum MeeVerlengenTypeEnum
    {
        [Description("Default")]
        Default,
        [Description("Tot ontruiming")]
        To,
        [Description("MK tot ontruiming")]
        MKTo,
        [Description("Voetganger")]
        Voetganger
    }
}
