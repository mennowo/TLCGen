using System.ComponentModel;
using TLCGen.Helpers;

namespace TLCGen.Dependencies.Models.Enumerations
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum CCOLCodeCaseEnum
    {
        [Description("Ongewijzigd")]
        OriginalCase = 0,
        [Description("Kleine letters")]
        LowerCase = 1,
        [Description("Hoofdletters")]
        UpperCase = 2
    }
}
