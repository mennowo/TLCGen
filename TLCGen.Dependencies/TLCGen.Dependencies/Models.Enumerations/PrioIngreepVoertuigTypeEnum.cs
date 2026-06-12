using System.ComponentModel;
using TLCGen.Helpers;

namespace TLCGen.Models.Enumerations
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum PrioIngreepVoertuigTypeEnum
    {
        Tram = 0,
        Bus = 1,
        Fiets = 2,
        Vrachtwagen = 3,
        Auto = 4,
        NG = 5,
        [Description("Nood- en hulpdienst")]
        Hulpdienst = 6,
    }
}
