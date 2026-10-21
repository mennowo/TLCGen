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
                            if (ov.MeldingenData.Wissel1Type == PrioIngreepInUitDataWisselTypeEnum.Ingang)
                            {
                                sb.Append(ov.MeldingenData.Wissel1InputVoorwaarde ?
                                    $"((SCH[{_schpf}{_schwisselpol}{ov.MeldingenData.Wissel1Input}] ? !IS[{_ispf}{ov.MeldingenData.Wissel1Input}] : IS[{_ispf}{ov.MeldingenData.Wissel1Input}]) || SCH[{_schpf}{_schgeenwissel}{ov.MeldingenData.Wissel1Input}])" :
                                    $"((SCH[{_schpf}{_schwisselpol}{ov.MeldingenData.Wissel1Input}] ? IS[{_ispf}{ov.MeldingenData.Wissel1Input}] : !IS[{_ispf}{ov.MeldingenData.Wissel1Input}]) || SCH[{_schpf}{_schgeenwissel}{ov.MeldingenData.Wissel1Input}])");
                            }
                            else
                            {
                                sb.Append(ov.MeldingenData.Wissel1InputVoorwaarde ?
                                    $"((SCH[{_schpf}{_schwisselpol}{ov.MeldingenData.Wissel1Detector}] ? !D[{_dpf}{ov.MeldingenData.Wissel1Detector}] : D[{_dpf}{ov.MeldingenData.Wissel1Detector}]) || SCH[{_schpf}{_schgeenwissel}{ov.MeldingenData.Wissel1Detector}])" :
                                    $"((SCH[{_schpf}{_schwisselpol}{ov.MeldingenData.Wissel1Detector}] ? D[{_dpf}{ov.MeldingenData.Wissel1Detector}] : !D[{_dpf}{ov.MeldingenData.Wissel1Detector}]) || SCH[{_schpf}{_schgeenwissel}{ov.MeldingenData.Wissel1Detector}])");
                            }
                        }
                        if (ov.MeldingenData.Wissel2)
                        {
                            if (ov.MeldingenData.Wissel1)
                            {
                                sb.Append(" && ");
                            }
                            if (ov.MeldingenData.Wissel2Type == PrioIngreepInUitDataWisselTypeEnum.Ingang)
                            {
                                sb.Append(ov.MeldingenData.Wissel2InputVoorwaarde ?
                                    $"((SCH[{_schpf}{_schwisselpol}{ov.MeldingenData.Wissel2Input}] ? !IS[{_ispf}{ov.MeldingenData.Wissel2Input}] : IS[{_ispf}{ov.MeldingenData.Wissel2Input}]) || SCH[{_schpf}{_schgeenwissel}{ov.MeldingenData.Wissel2Input}])" :
                                    $"((SCH[{_schpf}{_schwisselpol}{ov.MeldingenData.Wissel2Input}] ? IS[{_ispf}{ov.MeldingenData.Wissel2Input}] : !IS[{_ispf}{ov.MeldingenData.Wissel2Input}]) || SCH[{_schpf}{_schgeenwissel}{ov.MeldingenData.Wissel2Input}])");
                            }
                            else
                            {
                                sb.Append(ov.MeldingenData.Wissel2InputVoorwaarde ?
                                    $"((SCH[{_schpf}{_schwisselpol}{ov.MeldingenData.Wissel2Detector}] ? !D[{_dpf}{ov.MeldingenData.Wissel2Detector}] : D[{_dpf}{ov.MeldingenData.Wissel2Detector}]) || SCH[{_schpf}{_schgeenwissel}{ov.MeldingenData.Wissel2Detector}])" :
                                    $"((SCH[{_schpf}{_schwisselpol}{ov.MeldingenData.Wissel2Detector}] ? D[{_dpf}{ov.MeldingenData.Wissel2Detector}] : !D[{_dpf}{ov.MeldingenData.Wissel2Detector}]) || SCH[{_schpf}{_schgeenwissel}{ov.MeldingenData.Wissel2Detector}])");
                            }
                        }
                        sb.AppendLine(");");
                    }
                }
            }
            return sb.ToString();
        }
    }
}
