using System.ComponentModel;
using TLCGen.Helpers;

namespace TLCGen.Models.Enumerations
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
	public enum HalfstarVARegelenTypeEnum
	{
        [Description("ML")]
		ML,
        [Description("Versneld PL")]
		VersneldPL
	}
}