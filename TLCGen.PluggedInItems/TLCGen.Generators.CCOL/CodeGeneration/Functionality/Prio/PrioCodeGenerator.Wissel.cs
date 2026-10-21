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
    // Prio: wissel conditions per ingreep, plus wagennummer buffer cleanup and the minimale roodtijd timers.
    // Hook: RegCPreApplication (reg.c PreApplication()).
    public partial class PrioCodeGenerator
    {
        private string GetPreApplicationCode(ControllerModel c, string ts)
        {
            var sb = new StringBuilder();

            var start = true;
            if(c.PrioData.PrioIngrepen.Any(x => x.CheckWagenNummer))
            {
                sb.AppendLine($"{ts}/* Opschonen wagennummer buffers */");
                sb.AppendLine($"{ts}WDNST_cleanup();");
                start = false;
            }
            if (c.PrioData.PrioIngrepen.Any(ov => ov.MeldingenData.Inmeldingen.Any(x => x.AlleenIndienRood) || ov.NoodaanvraagKoplus))
            {
                if (!start) sb.AppendLine();
                sb.AppendLine($"{ts}/* Herstarting meting roodtijd tbv minimale roodtijd OV */");
                foreach (var ov in c.PrioData.PrioIngrepen)
                {
                    if (ov.MeldingenData.Inmeldingen.Any(x => x.AlleenIndienRood) || ov.NoodaanvraagKoplus)
                    {
                        sb.AppendLine($"{ts}RT[{_tpf}{_tovminrood}{CCOLCodeHelper.GetPriorityName(c, ov)}] = !R[{_fcpf}{ov.FaseCyclus}];");
                    }
                }
                start = false;
            }
            if (c.PrioData.PrioIngrepen.Any(x => x.HasOVIngreepWissel()))
            {
                sb.AppendLine($"{ts}/* Onthouden wissel voorwaarden per fasecyclus */");
                foreach (var ov in c.PrioData.PrioIngrepen)
                {
                    if (ov.HasOVIngreepWissel())
                    {
                        sb.Append($"{ts}IH[{_hpf}{_hwissel}{CCOLCodeHelper.GetPriorityName(c, ov)}] = (");
                        if (ov.MeldingenData.Wissel1)
                        {
                            sb.Append(GetWisselCondition(ov.MeldingenData.Wissel1Type, ov.MeldingenData.Wissel1Input, ov.MeldingenData.Wissel1Detector, ov.MeldingenData.Wissel1InputVoorwaarde));
                        }
                        if (ov.MeldingenData.Wissel2)
                        {
                            if (ov.MeldingenData.Wissel1)
                            {
                                sb.Append(" && ");
                            }
                            sb.Append(GetWisselCondition(ov.MeldingenData.Wissel2Type, ov.MeldingenData.Wissel2Input, ov.MeldingenData.Wissel2Detector, ov.MeldingenData.Wissel2InputVoorwaarde));
                        }
                        sb.AppendLine(");");
                    }
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Condition of one wissel: the input (IS) or detector (D) is in the required state (inverted by the polarity
        /// switch schwisselpol), or the wissel is not used (schgeenwissel).
        /// </summary>
        /// <param name="voorwaarde">Required state: TRUE when the input or detector must be high.</param>
        private string GetWisselCondition(PrioIngreepInUitDataWisselTypeEnum type, string input, string detector, bool voorwaarde)
        {
            var ingang = type == PrioIngreepInUitDataWisselTypeEnum.Ingang;
            var name = ingang ? input : detector;
            var state = ingang ? $"IS[{_ispf}{name}]" : $"D[{_dpf}{name}]";
            var whenPolarity = voorwaarde ? $"!{state}" : state;
            var otherwise = voorwaarde ? state : $"!{state}";
            return $"((SCH[{_schpf}{_schwisselpol}{name}] ? {whenPolarity} : {otherwise}) || SCH[{_schpf}{_schgeenwissel}{name}])";
        }
    }
}
