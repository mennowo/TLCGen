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
    public enum MeeaanvraagTypeEnum
    {
        [Description("Aanvraag")]
        Aanvraag,
        [Description("Rood voor aanvraag")]
        RoodVoorAanvraag,
        [Description("RA geen conflicten")]
        RoodVoorAanvraagGeenConflicten,
        [Description("Startgroen")]
        Startgroen
    }
}
