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
        //[Description("Default")]
        [Description("ymmax")]
        Default,
        //[Description("Tot ontruiming")]
        [Description("ymmax_to")]
        To,
        //[Description("MK tot ontruiming")]
        [Description("ymmax || MK[fc] && ymmax_to")]
        MKTo,
        //[Description("Voetganger")]
        [Description("ymmax_vtg")]
        Voetganger
    }
}
