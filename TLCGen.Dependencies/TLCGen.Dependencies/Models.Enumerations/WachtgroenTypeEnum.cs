using System.ComponentModel;
using TLCGen.Helpers;

namespace TLCGen.Models.Enumerations;

[TypeConverter(typeof(EnumDescriptionTypeConverter))]
public enum WachtgroenTypeEnum
{
    [Description("Geen wachtgroen")]
    Geen = 0,
    [Description("Groen vasthouden")]
    GroenVasthouden = 1,
    [Description("Groen vasthouden en aanvragen")]
    GroenVasthoudenEnAanvragen = 2,
}