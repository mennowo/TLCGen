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
    // Prio: post processing after the prio functions: blocking during HD, and preventing afkappen during fixatie,
    // hard meeverlengen, nalopen and late release.
    // Hook: PrioCPostAfhandelingPrio (prio.c PostAfhandelingPrio()).
    public partial class PrioCodeGenerator
    {
        private string GetPostAfhandelingPrioCode(ControllerModel c, string ts)
        {
            var sb = new StringBuilder();
            var first = false;

            if (c.HasHD() && c.PrioData.BlokkeerNietConflictenBijHDIngreep)
            {
                sb.AppendLine($"{ts}/* Bepalen of een HD ingreep actief is */");
                sb.Append($"{ts}isHD = ");
                first = true;
                foreach (var hd in c.PrioData.HDIngrepen)
                {
                    if (!first)
                    {
                        sb.Append(" || ");
                    }
                    sb.Append($"C[{_ctpf}{_cvchd}{hd.FaseCyclus}] && !BL[{_fcpf}{hd.FaseCyclus}]");
                    first = false;
                }
                foreach (var prio in c.PrioData.PrioIngrepen.Where(x => x.Type == PrioIngreepVoertuigTypeEnum.Hulpdienst))
                {
                    if (!first)
                    {
                        sb.Append(" || ");
                    }
                    sb.Append($"C[{_ctpf}{_cvc}{CCOLCodeHelper.GetPriorityName(c, prio)}] && !BL[{_fcpf}{prio.FaseCyclus}]");
                    first = false;
                }
                sb.AppendLine(";");
                sb.AppendLine();
                if (!c.PrioData.BlokkeerNietConflictenAlleenLangzaamVerkeer)
                {
                    sb.AppendLine($"{ts}/* Blokkeren alle richtingen zonder HD ingreep */");
                    sb.AppendLine($"{ts}if (isHD)");
                    sb.AppendLine($"{ts}{{");
                    foreach (var fc in c.Fasen)
                    {
                        AppendBlokkeerBijHDCode(c, sb, fc.Naam, ts);
                    }
                }
                else
                {
                    sb.AppendLine($"{ts}/* Blokkeren alle langzaam verkeer (tevens niet-conflicten) */");
                    if(c.Fasen.Any(x => x.WachttijdVoorspeller))
                    {
                        sb.AppendLine($"{ts}/* Blokkeren uitstellen indien een wachttijdvoorspeller onder het minimum is */");
                        foreach(var fc in c.Fasen.Where(x => x.WachttijdVoorspeller))
                        {
                            sb.AppendLine($"{ts}isWTV |= (MM[{_mpf}{_mwtvm}{fc.Naam}] && MM[{_mpf}{_mwtvm}{fc.Naam}] <= PRM[{_prmpf}{_prmwtvnhaltmin}]);");
                        }
                        sb.AppendLine($"{ts}if (isHD && !isWTV)");
                    }
                    else
                    {
                        sb.AppendLine($"{ts}if (isHD)");
                    }
                    sb.AppendLine($"{ts}{{");
                    foreach (var fc in c.Fasen.Where(x => x.Type == FaseTypeEnum.Fiets || x.Type == FaseTypeEnum.Voetganger))
                    {
                        AppendBlokkeerBijHDCode(c, sb, fc.Naam, ts);
                    }
                }
                sb.AppendLine($"{ts}}}");
                sb.AppendLine();
            }

            if (c.Data.FixatieData.FixatieMogelijk)
            { 
                sb.AppendLine($"{ts}/* Niet afkappen tijdens fixeren */");
                sb.AppendLine($"{ts}if (IS[isfix])");
                sb.AppendLine($"{ts}{{");
                sb.AppendLine($"{ts}{ts}for (fc = 0; fc < FCMAX; ++fc)");
                sb.AppendLine($"{ts}{ts}{{");
                sb.AppendLine($"{ts}{ts}{ts}Z[fc] &= ~PRIO_Z_BIT;");
                sb.AppendLine($"{ts}{ts}{ts}FM[fc] &= ~PRIO_FM_BIT;");
                sb.AppendLine($"{ts}{ts}}}");
                sb.AppendLine($"{ts}}}");
                sb.AppendLine();
            }
            if (c.Fasen.Any(x => x.HardMeeverlengenFaseCycli.Count > 0))
            {
                sb.AppendLine($"{ts}/* Niet afkappen hard meeverlengen */");
                sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
                sb.AppendLine($"{ts}{{");
                sb.AppendLine($"{ts}{ts}if (YM[fc] & BIT1)");
                sb.AppendLine($"{ts}{ts}{{");
                sb.AppendLine($"{ts}{ts}{ts}Z[fc] &= ~PRIO_Z_BIT;");
                sb.AppendLine($"{ts}{ts}{ts}FM[fc] &= ~PRIO_FM_BIT;");
                sb.AppendLine($"{ts}{ts}}}");
                sb.AppendLine($"{ts}}}");
                sb.AppendLine();
            }

            if (c.InterSignaalGroep.Nalopen.Any())
            {
                sb.AppendLine($"{ts}/* Niet afkappen naloop richtingen wanneer een naloop tijd nog loopt */");
                foreach (var fc in c.Fasen)
                {
                    var found = false; 
                    first = true;
                    foreach (var nl in c.InterSignaalGroep.Nalopen.Where(x => x.FaseNaar == fc.Naam))
                    {
                        if (!found)
                        {
                            sb.Append($"{ts}if (");
                        }
                        found = true;
                        foreach (var nlt in nl.Tijden)
                        {
                            if (!first) sb.Append(" || ");
                            first = false;
                            var tnl = nlt.Type switch
                            {
                                NaloopTijdTypeEnum.StartGroen => _tnlsg,
                                NaloopTijdTypeEnum.StartGroenDetectie => _tnlsgd,
                                NaloopTijdTypeEnum.VastGroen => _tnlfg,
                                NaloopTijdTypeEnum.VastGroenDetectie => _tnlfgd,
                                NaloopTijdTypeEnum.EindeGroen => _tnleg,
                                NaloopTijdTypeEnum.EindeGroenDetectie => _tnlegd,
                                NaloopTijdTypeEnum.EindeVerlengGroen => _tnlcv,
                                NaloopTijdTypeEnum.EindeVerlengGroenDetectie => _tnlcvd,
                                _ => throw new ArgumentOutOfRangeException()
                            };
                            sb.Append($"RT[{_tpf}{tnl}{nl.FaseVan}{nl.FaseNaar}] || T[{_tpf}{tnl}{nl.FaseVan}{nl.FaseNaar}]");
                        }
                    }

                    if (found)
                    {
                        sb.AppendLine(")");
                        sb.AppendLine($"{ts}{{");
                        sb.AppendLine($"{ts}{ts}Z[{_fcpf}{fc.Naam}] &= ~BIT6;");
                        sb.AppendLine(c.Data.SynchronisatiesType == SynchronisatiesTypeEnum.RealFunc 
                            ? $"{ts}{ts}RR[{_fcpf}{fc.Naam}] &= ~(BIT1 | BIT2 | BIT4 | BIT5 | BIT6);" 
                            : $"{ts}{ts}RR[{_fcpf}{fc.Naam}] &= ~BIT6;");
                        sb.AppendLine($"{ts}{ts}FM[{_fcpf}{fc.Naam}] &= ~PRIO_FM_BIT;");
                        sb.AppendLine($"{ts}}}");
                    }
                }
            }
            if (c.InterSignaalGroep.LateReleases.Any())
            {
                sb.AppendLine();
                sb.AppendLine($"{ts}/* Niet afkappen late release richtingen wanneer de laterelease tijd nog loopt */");
                foreach (var lr in c.InterSignaalGroep.LateReleases)
                {
                    var t = $"{_tpf}{_treallr}{lr}";
                    sb.AppendLine($"{ts}RT[{t}] = ER[{_fcpf}{lr:naar}];");

                    sb.AppendLine($"{ts}if (RT[{t}] || T[{t}])");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}Z[{_fcpf}{lr:van}] &= ~BIT6;");
                    sb.AppendLine($"{ts}{ts}RR[{_fcpf}{lr:van}] &= ~(BIT1 | BIT2 | BIT4 | BIT5 | BIT6);");
                    sb.AppendLine($"{ts}{ts}FM[{_fcpf}{lr:van}] &= ~PRIO_FM_BIT;");
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine();
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Blocks a fase during a HD ingreep, unless the fase has a HD ingreep of its own that is active.
        /// The counters of both legacy (cvchd&lt;fc&gt;) and generic (cvc&lt;prio&gt;) HD ingrepen count.
        /// </summary>
        private void AppendBlokkeerBijHDCode(ControllerModel c, StringBuilder sb, string fc, string ts)
        {
            var counters = new List<string>();
            if (c.PrioData.HDIngrepen.Any(x => x.FaseCyclus == fc))
            {
                counters.Add($"{_ctpf}{_cvchd}{fc}");
            }
            counters.AddRange(c.PrioData.PrioIngrepen
                .Where(x => x.Type == PrioIngreepVoertuigTypeEnum.Hulpdienst && x.FaseCyclus == fc)
                .Select(x => $"{_ctpf}{_cvc}{CCOLCodeHelper.GetPriorityName(c, x)}"));

            var block = $"RR[{_fcpf}{fc}] |= BIT6; Z[{_fcpf}{fc}] |= BIT6;";
            if (counters.Count == 0)
            {
                sb.AppendLine($"{ts}{ts}{block}");
            }
            else
            {
                sb.AppendLine($"{ts}{ts}if ({string.Join(" && ", counters.Select(x => $"!C[{x}]"))}) {{ {block} }}");
            }
        }
    }
}
