using System.ComponentModel;
using TLCGen.Helpers;

namespace TLCGen.Models.Enumerations
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum SynchronisatiesTypeEnum
    {
        [Description("Realfunc")]
        RealFunc,
        [Browsable(false)]
        [Description("Interfunc")]
        InterFunc
    }
}