using System.ComponentModel;
using TLCGen.Helpers;

namespace TLCGen.Models.Enumerations
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum PrioIngreepTypeEnum
    {
        [Description("Geen OV module")]
        Geen,
        [Description("Generieke prioriteit")]
        GeneriekePrioriteit
    }
}
