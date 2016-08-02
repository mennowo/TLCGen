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
    public enum VLOGTypeEnum
    {
        [Description("Geen")]
        Geen,
        [Description("Streaming")]
        Streaming,
        [Description("Filebased")]
        Filebased
    }
}
