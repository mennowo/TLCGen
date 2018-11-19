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
    public enum OVIngreepPrioriteitEnum
    {
        [Description("Afkappen conflicten")]
        Afkappen = 1,
        [Description("Vasthouden groen")]
        Vasthouden = 2,
        [Description("Tussendoor realiseren")]
        Tussendoor = 3,
        [Description("Afkappen OV conflicten")]
        AfkappenOV = 4
    }
}
