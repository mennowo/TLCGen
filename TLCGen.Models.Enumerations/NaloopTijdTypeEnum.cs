using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models.Enumerations
{
    public enum NaloopTijdTypeEnum
    {
        [Description("Vastgroen")]
        VastGroen,
        [Description("Vastgr. + detectie")]
        VastGroenDetectie,
        [Description("Startgroen")]
        StartGroen,
        [Description("Startgr. + detectie")]
        StartGroenDetectie,
        [Description("Eindegroen")]
        EindeGroen,
        [Description("Eindegr. + detectie")]
        EindeGroenDetectie,
        [Description("Einde verlenggroen")]
        EindeVerlengGroen,
        [Description("Einde verl.gr. + detectie")]
        EindeVerlengGroenDetectie
    }
}
