using System.ComponentModel;
using TLCGen.Helpers;

namespace TLCGen.Models.Enumerations
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
	public enum HalfstarGekoppeldTypeEnum
	{
        [Description("Master")]
		Master,
        [Description("Slave")]
		Slave
	}
}