using System.ComponentModel;
using TLCGen.Helpers;

namespace TLCGen.GebruikersOpties
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum CCOLElementTypeEnum
    {
        TE_type,
        TS_type,
        TM_type,
        Geen,
    }
}
