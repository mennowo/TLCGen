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
    // Prio: verklikking (outputs) of ingrepen, KAR and maximum waiting time, and the prio counter.
    // Hooks: RegCSystemApplication and RegCPostSystemApplication (reg.c system_application()).
    public partial class PrioCodeGenerator
    {
        private string GetSystemApplicationCode(ControllerModel c, string ts)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"{ts}/* PRIO verklikking */");
            sb.AppendLine($"{ts}/* ---------------- */");
            if (!c.PrioData.PrioUitgangPerFase)
            {
                foreach (var ov in c.PrioData.PrioIngrepen)
                {
                    sb.AppendLine(
                        $"{ts}CIF_GUS[{_uspf}{_usovinm}{CCOLCodeHelper.GetPriorityName(c, ov)}] = C[{_ctpf}{_cvc}{CCOLCodeHelper.GetPriorityName(c, ov)}];");
                }
            }
            else
            {
                foreach (var sg in c.Fasen.Where(x => x.PrioIngreep))
                {
                    sb.Append(
                        $"{ts}CIF_GUS[{_uspf}{_usovinm}{sg.Naam}] = ");
                    var firstSg = true;
                    foreach (var ov in c.PrioData.PrioIngrepen.Where(x => x.FaseCyclus == sg.Naam))
                    {
                        if (!firstSg) sb.Append(" || ");
                        sb.Append($"C[{_ctpf}{_cvc}{CCOLCodeHelper.GetPriorityName(c, ov)}]");
                        firstSg = false;
                    }
                    sb.AppendLine(";");
                }
            }
            foreach (var hd in c.PrioData.HDIngrepen)
            {
                sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_ushdinm}{hd.FaseCyclus}] = C[{_ctpf}{_cvchd}{hd.FaseCyclus}];");
            }
            if (c.HasKAR())
            {
                sb.AppendLine();
                sb.AppendLine($"{ts}/* Verklikken melding en ondergedrag KAR */");
                sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_uskarmelding}] = T[{_tpf}{_tkarmelding}];");
                sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_uskarog}] = !T[{_tpf}{_tkarog}];");
            }
            if (c.HasPT() || c.HasHD())
            {
                sb.AppendLine();
                sb.AppendLine($"{ts}/* Verklikken overschreiding maximale wachttijd */");
                sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_usmaxwt}] = FALSE;");
                sb.AppendLine($"{ts}for (ov = 0; ov < prioFCMAX; ++ov)");
                sb.AppendLine($"{ts}{ts}CIF_GUS[{_uspf}{_usmaxwt}] |= iMaximumWachtTijdOverschreden[ov];");
            }
            return sb.ToString();
        }

        private string GetPostSystemApplicationCode(ControllerModel c, string ts)
        {
            var sb = new StringBuilder();

            if (c.PrioData.VerklikkenPrioTellerUber == NooitAltijdAanUitEnum.Nooit) return "";
            sb.AppendLine($"{ts}/* Verklikken wijzigingen OV-teller */");
            var sch = c.PrioData.VerklikkenPrioTellerUber == NooitAltijdAanUitEnum.Altijd ? "NG" : $"{_schpf}{_schcovuber}";
            foreach (var ov in c.PrioData.PrioIngrepen)
            {
                sb.AppendLine($"{ts}PRIO_teller({_ctpf}{_cvc}{CCOLCodeHelper.GetPriorityName(c, ov)}, {sch});");
            }
            foreach (var hd in c.PrioData.HDIngrepen)
            {
                sb.AppendLine($"{ts}PRIO_teller({_ctpf}{_cvchd}{hd.FaseCyclus}, {sch});");
            }
            return sb.ToString();
        }
    }
}
