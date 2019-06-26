using System;
using System.Collections.Generic;
using System.Linq;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public static class CCOLElementCollector
    {
        #region Static Fields

        private static Dictionary<string, Dictionary<int, int>> _koppelSignaalCountPerFunc = new Dictionary<string, Dictionary<int, int>>();
        private static Dictionary<string, bool> _koppelSignaalCountSet;
        private static Dictionary<string, List<CCOLKoppelSignaal>> _koppelSignalen;

        #endregion // Static Fields

        #region Static Methods

        public static void AddKoppelSignaal(string koppeling, int order, string name, CCOLKoppelSignaalRichtingEnum richting)
        {
            if (!_koppelSignaalCountPerFunc.ContainsKey(koppeling))
            {
                _koppelSignaalCountPerFunc.Add(koppeling, new Dictionary<int, int>());
            }
            if (!_koppelSignaalCountPerFunc[koppeling].ContainsKey(order))
            {
                _koppelSignaalCountPerFunc[koppeling].Add(order, 1);
            }
            if (!_koppelSignalen.ContainsKey(koppeling))
            {
                _koppelSignalen.Add(koppeling, new List<CCOLKoppelSignaal>());
                _koppelSignaalCountSet.Add(koppeling, false);
            }
            _koppelSignalen[koppeling].Add(new CCOLKoppelSignaal() { Count = _koppelSignaalCountPerFunc[koppeling][order], Order = order, Name = name, Richting = richting });
            ++_koppelSignaalCountPerFunc[koppeling][order];
        }

        public static void AddKoppelSignaal(string koppeling, int order, int count, string name, CCOLKoppelSignaalRichtingEnum richting)
        {
            if (!_koppelSignalen.ContainsKey(koppeling))
            {
                _koppelSignalen.Add(koppeling, new List<CCOLKoppelSignaal>());
                _koppelSignaalCountSet.Add(koppeling, false);
            }
            else if (_koppelSignalen[koppeling].Any(x => x.Count == count))
            {
                throw new IndexOutOfRangeException();
            }
            if (!_koppelSignaalCountPerFunc.ContainsKey(koppeling))
            {
                _koppelSignaalCountPerFunc.Add(koppeling, new Dictionary<int, int>());
            }
            if (!_koppelSignaalCountPerFunc[koppeling].ContainsKey(order))
            {
                _koppelSignaalCountPerFunc[koppeling].Add(order, 1);
            }
            if (count == 0)
            {
                count = _koppelSignaalCountPerFunc[koppeling][order];
            }
            _koppelSignalen[koppeling].Add(new CCOLKoppelSignaal() { Count = count, Order = order, Name = name, Richting = richting });
            ++_koppelSignaalCountPerFunc[koppeling][order];
        }

        public static int GetKoppelSignaalCount(string koppeling, ControllerModel c, string name, CCOLKoppelSignaalRichtingEnum richting)
        {
            if (_koppelSignaalCountSet.ContainsKey(koppeling) && 
                _koppelSignalen.ContainsKey(koppeling) && 
                !_koppelSignaalCountSet[koppeling])
            {
                _koppelSignalen[koppeling].Sort((x, y) => (x.Order * 1000 + x.Count).CompareTo(y.Order * 1000 + y.Count));
                for (int i = 1; i <= _koppelSignalen[koppeling].Count; i++)
                {
                    _koppelSignalen[koppeling][i - 1].CountAll = i;
                }
                _koppelSignaalCountSet[koppeling] = true;
            }
            var ks = _koppelSignalen[koppeling].FirstOrDefault(x => x.Name == name && x.Richting == richting);
            if (ks == null) return 0;
            var ct = ks.CountAll;
            switch (ks.Richting)
            {
                case CCOLKoppelSignaalRichtingEnum.In:
                    var kop = c.PTPData.PTPKoppelingen.FirstOrDefault(x => x.TeKoppelenKruispunt == koppeling);
                    //if (kop.StartIndexInkomendeKoppelSignalen > 1) ct += kop.StartIndexInkomendeKoppelSignalen - 1;
                    break;
                case CCOLKoppelSignaalRichtingEnum.Uit:
                    var kop2 = c.PTPData.PTPKoppelingen.FirstOrDefault(x => x.TeKoppelenKruispunt == koppeling);
                    //if (kop2.StartIndexUitgaandeKoppelSignalen > 1) ct += kop2.StartIndexUitgaandeKoppelSignalen - 1;
                    break;
            }
            return ct;
        }

        #endregion // Static Methods

        #region Static Public Methods

        public static void Reset()
        {
            _koppelSignaalCountPerFunc.Clear();
            _koppelSignaalCountSet = new Dictionary<string, bool>();
            _koppelSignalen = new Dictionary<string, List<CCOLKoppelSignaal>>();
        }

        public static void AddAllMaxElements(CCOLElemListData[] lists)
        {
            lists[0].Elements.Add(new CCOLElement() { Define = "USMAX1", Commentaar = "Totaal aantal uitgangen" });
            lists[1].Elements.Add(new CCOLElement() { Define = "ISMAX1", Commentaar = "Totaal aantal ingangen" });
            lists[2].Elements.Add(new CCOLElement() { Define = "HEMAX1", Commentaar = "Totaal aantal hulpelementen" });
            lists[3].Elements.Add(new CCOLElement() { Define = "MEMAX1", Commentaar = "Totaal aantal geheugen elementen" });
            lists[4].Elements.Add(new CCOLElement() { Define = "TMMAX1", Commentaar = "Totaal aantal timers" });
            lists[5].Elements.Add(new CCOLElement() { Define = "CTMAX1", Commentaar = "Totaal aantal counters" });
            lists[6].Elements.Add(new CCOLElement() { Define = "SCHMAX1", Commentaar = "Totaal aantal schakelaars" });
            lists[7].Elements.Add(new CCOLElement() { Define = "PRMMAX1", Commentaar = "Totaal aantal parameters" });
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
                if (pgen.HasCCOLElements())
                {
                    foreach (var i in pgen.GetCCOLElements(CCOLElementTypeEnum.Uitgang))
                    {
                        data.Elements.Add(i);
                    }
                }
            }
            
            return data;
        }

        private static CCOLElemListData CollectAllIngangen(ControllerModel controller, List<ICCOLCodePieceGenerator> pgens)
        {
            var data = new CCOLElemListData { CCOLCode = "IS_code" };

            foreach (var pgen in pgens)
            {
                if (pgen.HasCCOLElements())
                {
                    foreach (var i in pgen.GetCCOLElements(CCOLElementTypeEnum.Ingang))
                    {
                        data.Elements.Add(i);
                    }
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
                if (pgen.HasCCOLElements())
                {
                    foreach (var i in pgen.GetCCOLElements(CCOLElementTypeEnum.HulpElement))
                    {
                        data.Elements.Add(i);
                    }
                }
            }

            if (data.Elements.Count == 0)
            {
                data.Elements.Add(new CCOLElement() { Define = "hedummy", Naam = "dummy" });
            }

            return data;
        }

        private static CCOLElemListData CollectAllGeheugenElementen(ControllerModel controller, List<ICCOLCodePieceGenerator> pgens)
        {
            var data = new CCOLElemListData { CCOLCode = "MM_code" };

            data.Elements.Add(new CCOLElement() { Define = "mperiod", Naam = "PERIOD", Commentaar = "Onthouden actieve periode" });

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
                data.Elements.Add(new CCOLElement { Define = "tdummy", Naam = "dummy" });
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
                data.Elements.Add(new CCOLElement() { Define = "ctdummy", Naam = "dummy" });
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
            data.Elements.Add(new CCOLElement() { Define = "prmfb", Naam = "FB", Instelling = controller.Data.Fasebewaking, TType = CCOLElementTimeTypeEnum.TS_type, Commentaar = "Instelling fasebewaking" });

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
