using System.ComponentModel;
using TLCGen.Helpers;

namespace TLCGen.Models.Enumerations
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
	public enum HalfstarGekoppeldWijzeEnum
	{
        [Description("PTP")]
		PTP,
        [Description("Koppelsignalen")]
        Koppelsignalen
	}
}