using System;
using System.Collections.Generic;
using System.Linq;
using TLCGen.Extensions;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public static class CCOLCodeHelper
    {
        public static List<List<string>> GetSyncGroupsForController(ControllerModel c)
        {
            var syncData = GroenSyncDataModel.ConvertSyncFuncToRealFunc(c);
            
            var syncGroups = new List<List<IInterSignaalGroepElement>>();

            foreach (var sync in syncData.GroenSyncFasen)
            {
                var existing = syncGroups.FirstOrDefault(x => x.Any(x2 => x2.FaseNaar == sync.FaseVan));
                if (existing != null)
                {
                    existing.Add(sync);
                    continue;
                }
                syncGroups.Add(new List<IInterSignaalGroepElement>{ sync });
            }
            
            var syncGroups1 = new List<List<IInterSignaalGroepElement>>();
            foreach (var group in syncGroups)
            {
                var existing = syncGroups1.FirstOrDefault(x => x.Any(x2 => group.Any(x3 => x2.FaseVan == x3.FaseNaar)));
                if (existing != null)
                {
                    existing.AddRange(group);
                    continue;
                }
                syncGroups1.Add(group);
            }

            return syncGroups1
                .Select(x => x
                    .Select(x2 => x2.FaseVan)
                    .Concat(x
                        .Select(x2 => x2.FaseNaar))
                    .Distinct()
                    .ToList())
                .ToList();
        }
        
        public static string GetPriorityName(ControllerModel c, PrioIngreepModel prio)
        {
            return prio.FaseCyclus + 
                (c.PrioData.WeglatenIngreepNaamBijEnkeleIngreepPerFase && c.PrioData.PrioIngrepen.Count(x => x.FaseCyclus == prio.FaseCyclus) == 1 
                ? "" 
                : prio.Naam);
        }

        public static bool HasSignalGroupConflictWithPT(ControllerModel c, string sgname)
        {
            foreach (var ov in c.PrioData.PrioIngrepen)
            {
                if (sgname == ov.FaseCyclus)
                    continue;

                foreach (var k in c.InterSignaalGroep.Conflicten)
                {
                    if (k.FaseVan == sgname && k.FaseNaar == ov.FaseCyclus)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static int GetAanvraagSetting(DetectorModel dm)
        {
            var set = dm.Aanvraag switch
            {
                DetectorAanvraagTypeEnum.Uit => 0,
                DetectorAanvraagTypeEnum.RnietTRG => 1,
                DetectorAanvraagTypeEnum.Rood => 2,
                DetectorAanvraagTypeEnum.RoodGeel => 3,
                _ => 0
            };
            return set;
        }

        public static List<Tuple<string, List<string>>> GetFasenWithGelijkStarts(ControllerModel c)
        {
            // Build a list of fasen with gelijkstarts belonging to them
            var gss = new List<Tuple<string, List<string>>>();

            if (!c.InterSignaalGroep.Gelijkstarten.Any()) return gss;

            foreach(var fc in c.Fasen)
            {
                if (c.InterSignaalGroep.Gelijkstarten.Any(x => x.FaseVan == fc.Naam || x.FaseNaar == fc.Naam) && !gss.Any(x => x.Item1 == fc.Naam))
                {
                    gss.Add(new Tuple<string, List<string>>(fc.Naam, new List<string>()));
                }
            }
            foreach (var i in gss)
            {
                i.Item2.Add(i.Item1);
            }
            foreach (var gs in c.InterSignaalGroep.Gelijkstarten)
            {
                // Add on both sides, cause the matrix is symmetrical, but gelijkstarts are only present in the model list once
                foreach(var i in gss)
                {
                    if(i.Item1 == gs.FaseVan)
                    {
                        i.Item2.Add(gs.FaseNaar);
                    }
                }
                foreach (var i in gss)
                {
                    if (i.Item1 == gs.FaseNaar)
                    {
                        i.Item2.Add(gs.FaseVan);
                    }
                }
            }
            foreach(var t in gss)
            {
                t.Item2.BubbleSort();
            }
            return gss;
        }
    }
}
