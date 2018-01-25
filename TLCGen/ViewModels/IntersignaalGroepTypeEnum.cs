using System;

namespace TLCGen.ViewModels.Enums
{
    [Flags]
    public enum IntersignaalGroepTypeEnum
    {
        Conflict = 0x1,
        GarantieConflict = 0x2,
        Gelijkstart = 0x4,
        Voorstart = 0x8,
        Naloop = 0x10,
        Meeaanvraag = 0x20,
        LateRelease = 0x40,
        SomeConflict = Conflict | GarantieConflict,
        SomeSynchronisatie = Gelijkstart | Voorstart | Naloop | Meeaanvraag | LateRelease
    }
}