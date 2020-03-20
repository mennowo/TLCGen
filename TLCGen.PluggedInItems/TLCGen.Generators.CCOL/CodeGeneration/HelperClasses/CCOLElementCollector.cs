using System.Collections.Generic;
using System.Linq;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public static class CCOLElementCollector
    {
        #region Static Fields

        private static Dictionary<string, List<CCOLKoppelSignaal>> _koppelSignalen;

        #endregion // Static Fields

        #region Static Methods

        public static void AddKoppelSignaal(string koppeling, int count, string name, KoppelSignaalRichtingEnum richting)
        {
            var koppelingKey = koppeling + richting.ToString();
            if (!_koppelSignalen.ContainsKey(koppelingKey))
            {
                _koppelSignalen.Add(koppelingKey, new List<CCOLKoppelSignaal>());
            }
            else if (count != 0 && _koppelSignalen[koppelingKey].Any(x => x.Count == count))
            {
                Dependencies.Providers.TLCGenDialogProvider.Default.ShowMessageBox(
                    $"{(richting == KoppelSignaalRichtingEnum.In ? "Ingangssignaal" : "Uitgangssignaal")} " +
                       $"nummer {count} van koppeling {koppeling} wordt reeds elders gebruikt. Dit kan de juiste werking " +
                       $"van de regeling negatief beinvloeden.", 
                    "Koppelsignaal dubbel gebruikt", 
                    System.Windows.MessageBoxButton.OK);
            }
            if (count == 0)
            {
                // TODO msg!
            }
            else
            {
                _koppelSignalen[koppelingKey].Add(new CCOLKoppelSignaal { Count = count, Name = name, Richting = richting });
            }
        }

        public static int GetKoppelSignaalCount(string koppeling, string name, KoppelSignaalRichtingEnum richting)
        {
            var koppelingKey = koppeling + richting;
            CCOLKoppelSignaal ks = null;
            if (_koppelSignalen.ContainsKey(koppelingKey))
            {
                ks = _koppelSignalen[koppelingKey].FirstOrDefault(x => x.Name == name && x.Richting == richting);
            }
            return ks?.Count ?? 0;
        }

        #endregion // Static Methods

        #region Static Public Methods

        public static void Reset()
        {
            _koppelSignalen = new Dictionary<string, List<CCOLKoppelSignaal>>();
        }

        public static void AddAllMaxElements(CCOLElemListData[] lists)
        {
            lists[0].Elements.Add(new CCOLElement { Define = "USMAX1", Commentaar = "Totaal aantal uitgangen" });
            lists[1].Elements.Add(new CCOLElement { Define = "ISMAX1", Commentaar = "Totaal aantal ingangen" });
            lists[2].Elements.Add(new CCOLElement { Define = "HEMAX1", Commentaar = "Totaal aantal hulpelementen" });
            lists[3].Elements.Add(new CCOLElement { Define = "MEMAX1", Commentaar = "Totaal aantal geheugen elementen" });
            lists[4].Elements.Add(new CCOLElement { Define = "TMMAX1", Commentaar = "Totaal aantal timers" });
            lists[5].Elements.Add(new CCOLElement { Define = "CTMAX1", Commentaar = "Totaal aantal counters" });
            lists[6].Elements.Add(new CCOLElement { Define = "SCHMAX1", Commentaar = "Totaal aantal schakelaars" });
            lists[7].Elements.Add(new CCOLElement { Define = "PRMMAX1", Commentaar = "Totaal aantal parameters" });
        }

        public static CCOLElemListData[] CollectAllCCOLElements(ControllerModel controller, List<ICCOLCodePieceGenerator> pgens)
        {
            var lists = new CCOLElemListData[8];

            lists[0] = CollectAllUitgangen(controller, pgens);
            lists[1] = CollectAllIngangen(controller, pgens);
            lists[2] = CollectAllHulpElementen(controller, pgens);
            lists[3] = CollectAllGeheugenElementen(controller, pgens);
            lists[4] = CollectAllTimers(controller, pgens);
            lists[5] = CollectAllCounters(controller, pgens);
            lists[6] = CollectAllSchakelaars(controller, pgens);
            lists[7] = CollectAllParameters(controller, pgens);

            return lists;
        }

        #endregion // Static Public Methods

        #region Static Private Methods

        private static CCOLElemListData CollectAllUitgangen(ControllerModel controller, List<ICCOLCodePieceGenerator> pgens)
        {
            var data = new CCOLElemListData { CCOLCode = "US_code" };

            foreach (var pgen in pgens)
            {
                if (!pgen.HasCCOLElements()) continue;
                foreach (var i in pgen.GetCCOLElements(CCOLElementTypeEnum.Uitgang))
                {
                    data.Elements.Add(i);
                }
            }
            
            return data;
        }

        private static CCOLElemListData CollectAllIngangen(ControllerModel controller, IEnumerable<ICCOLCodePieceGenerator> pgens)
        {
            var data = new CCOLElemListData { CCOLCode = "IS_code" };

            foreach (var pgen in pgens)
            {
                if (!pgen.HasCCOLElements()) continue;
                foreach (var i in pgen.GetCCOLElements(CCOLElementTypeEnum.Ingang))
                {
                    data.Elements.Add(i);
                }
            }

            return data;
        }

        private static CCOLElemListData CollectAllHulpElementen(ControllerModel controller, List<ICCOLCodePieceGenerator> pgens)
        {
            var data = new CCOLElemListData { CCOLCode = "H_code" };

            // Collect everything

            foreach (var pgen in pgens)
            {
                if (!pgen.HasCCOLElements()) continue;
                foreach (var i in pgen.GetCCOLElements(CCOLElementTypeEnum.HulpElement))
                {
                    data.Elements.Add(i);
                }
            }

            if (data.Elements.Count == 0)
            {
                var he = Settings.CCOLGeneratorSettingsProvider.Default.GetPrefix(Settings.CCOLGeneratorSettingTypeEnum.Parameter);
                data.Elements.Add(new CCOLElement() { Define = $"{he}dummy", Naam = "dummy" });
            }

            return data;
        }

        private static CCOLElemListData CollectAllGeheugenElementen(ControllerModel controller, List<ICCOLCodePieceGenerator> pgens)
        {
            var data = new CCOLElemListData { CCOLCode = "MM_code" };

            // Collect everything
            foreach (var pgen in pgens)
            {
                if (pgen.HasCCOLElements())
                {
                    foreach (var i in pgen.GetCCOLElements(CCOLElementTypeEnum.GeheugenElement))
                    {
                        data.Elements.Add(i);
                    }
                }
            }

            return data;
        }

        private static CCOLElemListData CollectAllTimers(ControllerModel controller, List<ICCOLCodePieceGenerator> pgens)
        {
            var data = new CCOLElemListData
            {
                CCOLCode = "T_code",
                CCOLSetting = "T_max",
                CCOLTType = "T_type"
            };

            foreach (var pgen in pgens)
            {
                if (pgen.HasCCOLElements())
                {
                    foreach (var i in pgen.GetCCOLElements(CCOLElementTypeEnum.Timer))
                    {
                        data.Elements.Add(i);
                    }
                }
            }

            if (data.Elements.Count == 0)
            {
                var tm = Settings.CCOLGeneratorSettingsProvider.Default.GetPrefix(Settings.CCOLGeneratorSettingTypeEnum.Timer);
                data.Elements.Add(new CCOLElement { Define = $"{tm}dummy", Naam = "dummy" });
            }

            return data;
        }

        private static CCOLElemListData CollectAllSchakelaars(ControllerModel controller, List<ICCOLCodePieceGenerator> pgens)
        {
            var data = new CCOLElemListData
            {
                CCOLCode = "SCH_code",
                CCOLSetting = "SCH"
            };

            foreach (var pgen in pgens)
            {
                if (pgen.HasCCOLElements())
                {
                    foreach (var i in pgen.GetCCOLElements(CCOLElementTypeEnum.Schakelaar))
                    {
                        data.Elements.Add(i);
                    }
                }
            }

            return data;
        }

        private static CCOLElemListData CollectAllCounters(ControllerModel controller, List<ICCOLCodePieceGenerator> pgens)
        {
            var data = new CCOLElemListData
            {
                CCOLCode = "C_code",
                CCOLSetting = "C_max",
                CCOLTType = "C_type"
            };

            // Collect everything
            foreach (var pgen in pgens)
            {
                if (pgen.HasCCOLElements())
                {
                    foreach (var i in pgen.GetCCOLElements(CCOLElementTypeEnum.Counter))
                    {
                        data.Elements.Add(i);
                    }
                }
            }

            if (data.Elements.Count == 0)
            {
                var ct = CCOL.Settings.CCOLGeneratorSettingsProvider.Default.GetPrefix(Settings.CCOLGeneratorSettingTypeEnum.Counter);
                data.Elements.Add(new CCOLElement { Define = $"{ct}dummy", Naam = "dummy" });
            }

            return data;
        }

        private static CCOLElemListData CollectAllParameters(ControllerModel controller, List<ICCOLCodePieceGenerator> pgens)
        {
            var data = new CCOLElemListData
            {
                CCOLCode = "PRM_code",
                CCOLSetting = "PRM",
                CCOLTType = "PRM_type"
            };

            // Collect everything
            foreach(var pgen in pgens)
            {
                if (pgen.HasCCOLElements())
                {
                    foreach (var i in pgen.GetCCOLElements(CCOLElementTypeEnum.Parameter))
                    {
                        data.Elements.Add(i);
                    }
                }
            }

            return data;
        }

        #endregion // Static Private Methods

    }
}
