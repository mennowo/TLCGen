using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Extensions;
using TLCGen.Generators.CCOL.CodeGeneration.HelperClasses;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Settings;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    // Prio: RIS display (xyprintf of the prio request buffers) in the test environment.
    // Hook: part of PrioCInUitMelden (prio.c InUitMelden()), see GetInUitMeldenCode.
    // The RIS meldingen themselves are in Meldingen.cs, their elements in Elements.cs.
    public partial class PrioCodeGenerator
    {
        private void AppendRisDisplayCode(ControllerModel c, StringBuilder sb, string ts)
        {
            var ovRis = c.PrioData.PrioIngrepen
                .Where(x => x.MeldingenData.Inmeldingen.Any(x2 => x2.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde) ||
                            x.MeldingenData.Uitmeldingen.Any(x2 => x2.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde))
                .ToList();
            if (ovRis.Any())
            {
                sb.AppendLine();
                /* printf buffers  */
                sb.AppendLine($"{ts}#if !defined AUTOMAAT && !defined AUTOMAAT_TEST && !defined NO_RIS && !defined VISSIM && !defined SUMO");
                sb.AppendLine($"{ts}if (display) {{");
                sb.AppendLine($"{ts}{ts}/* RIS_PRIOREQUEST_AP */");
                sb.AppendLine($"{ts}{ts}for (i = 0; i < 5; ++i) {{");
                sb.AppendLine($"{ts}{ts}{ts}xyprintf(32, i+2, \"                                                \");");
                sb.AppendLine($"{ts}{ts}}}");
                sb.AppendLine($"{ts}{ts}xyprintf(32, 0, \"RIS_PRIOREQUEST_AP                              \");");
                sb.AppendLine($"{ts}{ts}xyprintf(32, 1, \"sg  id--------------- seq.Nr-req.Type--routeName\");");
                sb.AppendLine($"{ts}{ts}for (i = 0; i < (RIS_PRIOREQUEST_AP_NUMBER <= 5 ? RIS_PRIOREQUEST_AP_NUMBER : 5); ++i) {{");
                sb.AppendLine($"{ts}{ts}{ts}xyprintf(32, i+2, \"%s\", RIS_PRIOREQUEST_AP[i].signalGroup);");
                sb.AppendLine($"{ts}{ts}{ts}xyprintf(36, i+2, \"%s\", RIS_PRIOREQUEST_AP[i].id);");
                sb.AppendLine($"{ts}{ts}{ts}xyprintf(55, i+2, \"%d\", RIS_PRIOREQUEST_AP[i].sequenceNumber);");
                sb.AppendLine($"{ts}{ts}{ts}xyprintf(62, i+2, \"%d\", RIS_PRIOREQUEST_AP[i].requestType);");
                sb.AppendLine($"{ts}{ts}{ts}xyprintf(72, i+2, \"%s\", RIS_PRIOREQUEST_AP[i].routeName);");
                sb.AppendLine($"{ts}{ts}}}");
                sb.AppendLine();
                sb.AppendLine($"{ts}{ts}/* RIS_PRIOREQUEST_EX_AP */");
                sb.AppendLine($"{ts}{ts}for (i = 0; i < 5; i++) {{");
                sb.AppendLine($"{ts}{ts}{ts}xyprintf(32, i+10, \"                                                \");");
                sb.AppendLine($"{ts}{ts}}}");
                sb.AppendLine($"{ts}{ts}xyprintf(32, 8, \"RIS_PRIOREQUEST_EX_AP                           \");");
                sb.AppendLine($"{ts}{ts}xyprintf(32, 9, \"    id--------------- seq.Nr-prioState-prCtrlState\");");
                sb.AppendLine($"{ts}{ts}for (i = 0; i < (RIS_PRIOREQUEST_EX_AP_NUMBER <= 5 ? RIS_PRIOREQUEST_EX_AP_NUMBER : 5); ++i) {{");
                sb.AppendLine($"{ts}{ts}{ts}xyprintf(36, i+10, \"%s\", RIS_PRIOREQUEST_EX_AP[i].id);");
                sb.AppendLine($"{ts}{ts}{ts}xyprintf(55, i+10, \"%d\", RIS_PRIOREQUEST_EX_AP[i].sequenceNumber);");
                sb.AppendLine($"{ts}{ts}{ts}xyprintf(62, i+10, \"%d\", RIS_PRIOREQUEST_EX_AP[i].prioState);");
                sb.AppendLine($"{ts}{ts}{ts}xyprintf(72, i+10, \"%d\", RIS_PRIOREQUEST_EX_AP[i].prioControlState);");
                sb.AppendLine($"{ts}{ts}}}");
                sb.AppendLine($"{ts}}}");
                sb.AppendLine();
                sb.AppendLine($"{ts}#endif /* #ifndef AUTOMAAT */");
                sb.AppendLine();
            }
        }
    }
}
