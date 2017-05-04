using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Helpers;

namespace TLCGen.Generators.CCOL.ProjectGeneration
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum VisualProjectTypeEnum
    {
        [Description("2010")]
        Visual2010,
        [Description("2013")]
        Visual2013,
        [Description("2010 VISSIM")]
        Visual2010Vissim,
        [Description("2013 VISSIM")]
        Visual2013Vissim
    }
}
