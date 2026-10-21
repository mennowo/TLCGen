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
    // Prio: holding back the feeding direction of a naloop when the naloop direction conflicts with a prio ingreep.
    // Hook: RegCRealisatieAfhandelingNaModules (reg.c RealisatieAfhandeling()).
    public partial class PrioCodeGenerator
    {
        private string GetNaloopPrioConflictCode(ControllerModel c, string ts)
        {
            var sb = new StringBuilder();
            var first = false;

            var naloopWithPrioConflicts = GetNaloopWithPrioConflicts(c);
            if (naloopWithPrioConflicts.Count == 0) return "";

            if (c.Data.SynchronisatiesType == SynchronisatiesTypeEnum.InterFunc) return "";

            sb.AppendLine($"{ts}/* Tegenhouden voedende richting, bij een conflicterende prio-ingreep van de nalooprichting */");
            sb.AppendLine($"{ts}/* Afzetten RR */");
            foreach (var nl in naloopWithPrioConflicts)
            {
                sb.Append($"{ts}if (");
                first = true;
                foreach (var conflict in nl.conflicts)
                {
                    if (!first)
                    {
                        sb.AppendLine(" &&");
                        sb.Append($"{ts}    ");
                    }

                    var prios = c.GetPrioIngrepen(conflict.FaseNaar).ToArray();
                    
                    var confYv = $"(G[{_fcpf}{conflict:naar}] || !(YV[{_fcpf}{conflict:naar}] & PRIO_YV_BIT) ";
                    var padding = confYv.Length + ts.Length + 7;
                    sb.Append(confYv);
                    if (prios.Any())
                    {
                        var first1 = true;
                        foreach (var prio in prios)
                        {
                            if (!first1)
                            {
                                sb.AppendLine(" &&");
                                sb.Append("".PadLeft(padding));
                            }
                            else
                            {
                                sb.Append("|| ");
                            }

                            sb.Append($"!(iPrioriteitsOpties[prioFC{CCOLCodeHelper.GetPriorityName(c, prio)}] & poBijzonderRealiseren)");
                            first1 = false;;
                        }
                    }
                    sb.Append(")");
                    first = false;
                }
                sb.AppendLine($") RR[{_fcpf}{nl.naloop:van}] &= ~BIT10;");
            }
            sb.AppendLine();

            sb.AppendLine($"{ts}/* Opzetten RR */");
            foreach (var nl in naloopWithPrioConflicts)
            {
                sb.Append($"{ts}if (");
                first = true;
                foreach (var conflict in nl.conflicts)
                {
                    if (!first)
                    {
                        sb.AppendLine(" ||");
                        sb.Append($"{ts}    ");
                    }
                    sb.Append($"((Z[{_fcpf}{nl.naloop:van}] & PRIO_Z_BIT) && (YV[{_fcpf}{conflict:naar}] & PRIO_YV_BIT) && !G[{_fcpf}{conflict:naar}])");
                    first = false;
                }
                sb.AppendLine($") RR[{_fcpf}{nl.naloop:van}] |= BIT10;");
            }
            return sb.ToString();
        }

        private static List<(NaloopModel naloop, List<ConflictModel> conflicts)> GetNaloopWithPrioConflicts(ControllerModel c)
        {
            var naloopWithPrioConflicts = new List<(NaloopModel naloop, List<ConflictModel> conflicts)>();
            foreach (var nl in c.InterSignaalGroep.Nalopen.Where(x => x.Type == NaloopTypeEnum.EindeGroen || x.Type == NaloopTypeEnum.CyclischVerlengGroen))
            {
                // search conflicts of naloop mrPrio (search nl 'to' and conflict 'from')
                var conflicts = c.InterSignaalGroep.Conflicten.Where(x => x.FaseVan == nl.FaseNaar);
                if (!conflicts.Any()) continue;
                // find out if conflict has prio (search conflict 'to')
                var prioConflicts = new List<ConflictModel>();
                foreach (var conflict in conflicts)
                {
                    var prio = c.PrioData.PrioIngrepen.Any(x => x.FaseCyclus == conflict.FaseNaar);
                    if (prio) prioConflicts.Add(conflict);
                }

                if (prioConflicts.Any()) naloopWithPrioConflicts.Add((nl, prioConflicts));
            }

            return naloopWithPrioConflicts;
        }
    }
}
