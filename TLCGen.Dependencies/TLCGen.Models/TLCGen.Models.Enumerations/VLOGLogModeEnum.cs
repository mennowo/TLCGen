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
    public enum VLOGLogModeEnum
    {
        [Description("Log naar buffer")]
        VLOGMODE_LOG_BUFFER = 0,
        [Description("Log naar UBER ASCII")]
        VLOGMODE_LOG_UBER_ASCII = 1,
        [Description("Log naar UBER binair")]
        VLOGMODE_LOG_UBER_BINAIR = 2,
        [Description("Log naar file binair")]
        VLOGMODE_LOG_FILE_BINAIR = 3,
        [Description("Log naar file ASCII")]
        VLOGMODE_LOG_FILE_ASCII = 4,
    }
}
