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
    public enum VLOGMonModeEnum
    {
        [Description("Monitor data binair")]
        Binair = 0,
        [Description("Monitor data  ASCII")]
        ASCII = 1
    }
}
