using System.ComponentModel;
using TLCGen.Helpers;

namespace TLCGen.Models.Enumerations
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum OVIngreepTypeEnum
    {
        [Description("Geen OV module")]
        Geen,
        [Description("Uitgebreide OV module")]
        Uitgebreid,
        [Description("Generieke prioriteit")]
        GeneriekePrioriteit
    }
}
