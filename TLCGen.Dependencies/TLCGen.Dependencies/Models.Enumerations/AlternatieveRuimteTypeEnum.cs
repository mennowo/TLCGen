using System.ComponentModel;
using TLCGen.Helpers;

namespace TLCGen.Models.Enumerations
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum AlternatieveRuimteTypeEnum
    {
        [Description("max_tar_to/tig")]
        MaxTarToTig,
        [Description("max_tar")]
        MaxTar,
        [Description("Real_Ruimte")]
        RealRuimte,
    }
}