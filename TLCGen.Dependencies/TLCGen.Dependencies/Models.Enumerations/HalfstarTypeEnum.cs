using System.ComponentModel;
using TLCGen.Helpers;

namespace TLCGen.Models.Enumerations
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
	public enum HalfstarTypeEnum
	{
        [Description("Master")]
		Master,
        [Description("Fallback master")]
		FallbackMaster,
        [Description("Slave")]
		Slave
	}
}