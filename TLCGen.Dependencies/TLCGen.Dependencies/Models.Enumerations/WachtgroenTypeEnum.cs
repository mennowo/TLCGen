using System.ComponentModel;
using TLCGen.Helpers;

namespace TLCGen.Models.Enumerations;

[TypeConverter(typeof(EnumDescriptionTypeConverter))]
public enum WachtgroenTypeEnum
{
    [Description("Groen vasthouden")]
    GroenVasthouden,
    [Description("Groen vasthouden en aanvragen")]
    GroenVasthoudenEnAanvragen,
}