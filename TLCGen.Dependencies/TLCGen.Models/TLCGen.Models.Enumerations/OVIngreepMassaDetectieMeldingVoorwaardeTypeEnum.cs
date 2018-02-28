using System.ComponentModel;
using TLCGen.Helpers;

namespace TLCGen.Models.Enumerations
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum OVIngreepMassaDetectieMeldingVoorwaardeTypeEnum
    {
        DetectorStart,
        DetectorBezet,
        DetectorEind,
        DetectorGeenStoring,
        DetectorStoring,
        WisselContactOp,
        WisselContactAf
    }
}
