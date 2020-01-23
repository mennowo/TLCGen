using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Helpers;

namespace TLCGen.Models.Enumerations
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum PrioIngreepInUitMeldingVoorwaardeInputOmgangMetStoringTypeEnum
    {
        [Description("Melding opvang wederzijds")]
        MeldingOpvangWederzijds,
        [Description("Melding op d1 bij storing d2")]
        MeldingD1StoringD2,
        [Description("Melding op d2 bij storing d1")]
        MeldingD2StoringD1,
        [Description("Geen melding")]
        GeenMelding,
    }
}
