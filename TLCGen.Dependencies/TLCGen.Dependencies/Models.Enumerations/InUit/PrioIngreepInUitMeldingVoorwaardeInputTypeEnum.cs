﻿using System.ComponentModel;
using TLCGen.Helpers;

namespace TLCGen.Models.Enumerations
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum PrioIngreepInUitMeldingVoorwaardeInputTypeEnum
    {
        [Description("Start detectie")]
        StartDetectie,
        [Description("Detectie op")]
        DetectieOp,
        [Description("Detectie bezet")]
        DetectieBezet,
        [Description("Start detectie bezet")]
        StartDetectieBezet,
        [Description("Einde detectie")]
        EindeDetectie,
        [Description("Einde detectie hiaat")]
        EindeDetectieHiaat,
        [Description("Start ingang")]
        StartIngang,
        [Description("Ingang hoog")]
        IngangHoog,
        [Description("Einde ingang")]
        EindeIngang,
    }

}
