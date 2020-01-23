using System.ComponentModel;
using TLCGen.Helpers;

namespace TLCGen.Models.Enumerations
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum PrioIngreepPrioriteitEnum
    {
        [Description("Afkappen conflicten")]
        Afkappen = 1,
        [Description("Vasthouden groen")]
        Vasthouden = 2,
        [Description("Tussendoor realiseren")]
        Tussendoor = 3,
        [Description("Afkappen PRIO conflicten")]
        AfkappenPrio = 4
    }
}
