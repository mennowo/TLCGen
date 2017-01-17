﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public static class CCOLElementCollector
    {
        #region Static Fields

        private static List<DetectorModel> AlleDetectoren;

        #endregion // Static Fields

        #region Static Public Methods

        public static CCOLElemListData[] CollectAllCCOLElements(ControllerModel controller)
        {
            AlleDetectoren = new List<DetectorModel>();
            foreach (FaseCyclusModel fcm in controller.Fasen)
            {
                foreach (DetectorModel dm in fcm.Detectoren)
                    AlleDetectoren.Add(dm);
            }
            foreach (DetectorModel dm in controller.Detectoren)
                AlleDetectoren.Add(dm);

            CCOLElemListData[] lists = new CCOLElemListData[8];

            lists[0] = CollectAllUitgangen(controller);
            lists[1] = CollectAllIngangen(controller);
            lists[2] = CollectAllHulpElementen(controller);
            lists[3] = CollectAllGeheugenElementen(controller);
            lists[4] = CollectAllTimers(controller);
            lists[5] = CollectAllCounters(controller);
            lists[6] = CollectAllSchakelaars(controller);
            lists[7] = CollectAllParameters(controller);

            return lists;
        }

        #endregion // Static Public Methods

        #region Static Private Methods

        private static CCOLElemListData CollectAllUitgangen(ControllerModel controller)
        {
            CCOLElemListData data = new CCOLElemListData();

            data.CCOLCode = "US_code";

            // Collect everything
            data.Elements.Add(new CCOLElement() { Define = "ussegm1", Naam = "segm1" });
            data.Elements.Add(new CCOLElement() { Define = "ussegm2", Naam = "segm2" });
            data.Elements.Add(new CCOLElement() { Define = "ussegm3", Naam = "segm3" });
            data.Elements.Add(new CCOLElement() { Define = "ussegm4", Naam = "segm4" });
            data.Elements.Add(new CCOLElement() { Define = "ussegm5", Naam = "segm5" });
            data.Elements.Add(new CCOLElement() { Define = "ussegm6", Naam = "segm6" });
            data.Elements.Add(new CCOLElement() { Define = "ussegm7", Naam = "segm7" });

            // Add last, nameless element for maximum #define
            data.Elements.Add(new CCOLElement() { Define = "USMAX" });

            data.SetMax();
            return data;
        }

        private static CCOLElemListData CollectAllIngangen(ControllerModel controller)
        {
            CCOLElemListData data = new CCOLElemListData();

            data.CCOLCode = "IS_code";

            // Collect everything
            data.Elements.Add(new CCOLElement() { Define = "isfix", Naam = "fix" });
            // TODO

            // Add last, nameless element for maximum #define
            data.Elements.Add(new CCOLElement() { Define = "ISMAX" });

            data.SetMax();
            return data;
        }

        private static CCOLElemListData CollectAllHulpElementen(ControllerModel controller)
        {
            CCOLElemListData data = new CCOLElemListData();

            data.CCOLCode = "H_code";

            // Collect everything
            data.Elements.Add(new CCOLElement() { Define = "hedummy", Naam = "dummy" });

            // TODO

            // Add last, nameless element for maximum #define
            data.Elements.Add(new CCOLElement() { Define = "HEMAX" });

            data.SetMax();
            return data;
        }

        private static CCOLElemListData CollectAllGeheugenElementen(ControllerModel controller)
        {
            CCOLElemListData data = new CCOLElemListData();

            data.CCOLCode = "MM_code";

            data.Elements.Add(new CCOLElement() { Define = "mperiod", Naam = "PERIOD" });

            // Collect everything
            // TODO

            // Add last, nameless element for maximum #define
            data.Elements.Add(new CCOLElement() { Define = "MEMAX" });

            data.SetMax();
            return data;
        }

        private static CCOLElemListData CollectAllTimers(ControllerModel controller)
        {
            CCOLElemListData data = new CCOLElemListData();

            data.CCOLCode = "T_code";
            data.CCOLSetting = "T_max";
            data.CCOLTType = "T_type";

            // Collect Kopmax
            foreach (FaseCyclusModel fcm in controller.Fasen)
            {
                bool HasKopmax = false;
                foreach (DetectorModel dm in fcm.Detectoren)
                {
                    if (dm.Verlengen == Models.Enumerations.DetectorVerlengenTypeEnum.Kopmax)
                    {
                        HasKopmax = true;
                        break;
                    }
                }
                if (HasKopmax)
                {
                    CCOLElement elem = new CCOLElement();
                    elem.Define = $"tkm{fcm.Naam}";
                    elem.Naam = $"KM{fcm.Naam}";
                    elem.Instelling = fcm.Kopmax.ToString();
                    elem.TType = "TE_type";

                    data.Elements.Add(elem);
                }
            }

            if (data.Elements.Count == 0)
            {
                data.Elements.Add(new CCOLElement() { Define = "tdummy", Naam = "dummy" });
            }

            // Add last, nameless element for maximum #define
            data.Elements.Add(new CCOLElement() { Define = "TMMAX" });

            data.SetMax();
            return data;
        }

        private static CCOLElemListData CollectAllSchakelaars(ControllerModel controller)
        {
            CCOLElemListData data = new CCOLElemListData();

            data.CCOLCode = "SCH_code";
            data.CCOLSetting = "SCH";

            data.Elements.Add(new CCOLElement() { Define = "schbmfix", Naam = "bmfix", Instelling = "1" });

            // Collect schwg
            foreach (FaseCyclusModel fcm in controller.Fasen)
            {
                if (fcm.VasteAanvraag != Models.Enumerations.NooitAltijdAanUitEnum.Nooit &&
                    fcm.VasteAanvraag != Models.Enumerations.NooitAltijdAanUitEnum.Altijd)
                {
                    data.Elements.Add(new CCOLElement()
                    {
                        Define = $"schca{fcm.Naam}",
                        Naam = $"CA{fcm.Naam}",
                        Instelling = fcm.VasteAanvraag == Models.Enumerations.NooitAltijdAanUitEnum.SchAan ? "1" : "0"
                    });
                }
            }
            foreach (FaseCyclusModel fcm in controller.Fasen)
            {
                if (fcm.Wachtgroen != Models.Enumerations.NooitAltijdAanUitEnum.Nooit &&
                    fcm.Wachtgroen != Models.Enumerations.NooitAltijdAanUitEnum.Altijd)
                {
                    data.Elements.Add(new CCOLElement()
                    {
                        Define = $"schwg{fcm.Naam}",
                        Naam = $"WG{fcm.Naam}",
                        Instelling = fcm.Wachtgroen == Models.Enumerations.NooitAltijdAanUitEnum.SchAan ? "1" : "0"
                    });
                }
            }
            foreach (FaseCyclusModel fcm in controller.Fasen)
            {
                if (fcm.Meeverlengen != Models.Enumerations.NooitAltijdAanUitEnum.Nooit &&
                    fcm.Meeverlengen != Models.Enumerations.NooitAltijdAanUitEnum.Altijd)
                {
                    data.Elements.Add(new CCOLElement()
                    {
                        Define = $"schmv{fcm.Naam}",
                        Naam = $"MV{fcm.Naam}",
                        Instelling = fcm.Meeverlengen == Models.Enumerations.NooitAltijdAanUitEnum.SchAan ? "1" : "0"
                    });
                }
            }

            // Alternatieven
            if (controller.ModuleMolen.LangstWachtendeAlternatief)
            {
                // alternatieven wel/niet toestaan
                foreach (FaseCyclusModel fcm in controller.Fasen)
                {
                    data.Elements.Add(new CCOLElement()
                    {
                        Define = $"schaltg{fcm.Naam}",
                        Naam = $"altg{fcm.Naam}",
                        Instelling = "1"
                    });
                }
            }

            // Add last, nameless element for maximum #define
            data.Elements.Add(new CCOLElement() { Define = "SCHMAX" });

            data.SetMax();
            return data;
        }

        private static CCOLElemListData CollectAllCounters(ControllerModel controller)
        {
            CCOLElemListData data = new CCOLElemListData();

            data.CCOLCode = "C_code";
            data.CCOLSetting = "C_max";
            data.CCOLTType = "C_type";

            // Collect everything
            data.Elements.Add(new CCOLElement() { Define = "ctdummy", Naam = "dummy" });

            // Add last, nameless element for maximum #define
            data.Elements.Add(new CCOLElement() { Define = "CTMAX" });

            data.SetMax();
            return data;
        }

        private static CCOLElemListData CollectAllParameters(ControllerModel controller)
        {
            CCOLElemListData data = new CCOLElemListData();

            data.CCOLCode = "PRM_code";
            data.CCOLSetting = "PRM";
            data.CCOLTType = "PRM_type";

            // Collect everything
            data.Elements.Add(new CCOLElement() { Define = "prmfb", Naam = "FB", Instelling = "240", TType = "TS_type" });

            // Detectie aanvraag functie
            foreach (DetectorModel dm in AlleDetectoren)
            {
                if (dm.Aanvraag == Models.Enumerations.DetectorAanvraagTypeEnum.Geen)
                    continue;

                int set = 0;
                switch (dm.Aanvraag)
                {
                    case Models.Enumerations.DetectorAanvraagTypeEnum.Uit:
                        set = 0;
                        break;
                    case Models.Enumerations.DetectorAanvraagTypeEnum.RnietTRG:
                        set = 1;
                        break;
                    case Models.Enumerations.DetectorAanvraagTypeEnum.Rood:
                        set = 2;
                        break;
                    case Models.Enumerations.DetectorAanvraagTypeEnum.RoodGeel:
                        set = 3;
                        break;
                }
                data.Elements.Add(new CCOLElement() { Define = $"prmd{dm.Naam}", Naam = $"d{dm.Naam}", Instelling = $"{set}", TType = "0" });
            }

            // Detectie verlengkriterium
            foreach (DetectorModel dm in AlleDetectoren)
            {
                if (dm.Verlengen == Models.Enumerations.DetectorVerlengenTypeEnum.Geen)
                    continue;

                int set = 0;
                switch (dm.Verlengen)
                {
                    case Models.Enumerations.DetectorVerlengenTypeEnum.Uit:
                        set = 0;
                        break;
                    case Models.Enumerations.DetectorVerlengenTypeEnum.Kopmax:
                        set = 1;
                        break;
                    //case Models.Enumerations.DetectorVerlengenTypeEnum.MK3:
                    //    set = 2;
                    //    break;
                    case Models.Enumerations.DetectorVerlengenTypeEnum.MK2:
                        set = 3;
                        break;
                }
                data.Elements.Add(new CCOLElement() { Define = $"prmmkd{dm.Naam}", Naam = $"mkd{dm.Naam}", Instelling = $"{set}", TType = "0" });
            }

            // Maxgroentijden
            foreach (GroentijdenSetModel mgset in controller.GroentijdenSets)
            {
                foreach (GroentijdModel mgm in mgset.Groentijden)
                {
                    FaseCyclusModel thisfcm = null;
                    foreach (FaseCyclusModel fcm in controller.Fasen)
                    {
                        if (fcm.Naam == mgm.FaseCyclus)
                        {
                            thisfcm = fcm;
                            break;
                        }
                    }
                    if (thisfcm == null)
                        throw new NotImplementedException($"Maxgroentijd voor niet bestaande fase {mgm.FaseCyclus} opgegeven.");

                    data.Elements.Add(new CCOLElement()
                    {
                        Define = $"prm{mgset.Naam.ToLower()}{thisfcm.Naam}",
                        Naam = $"mk{mgset.Naam.ToLower()}{thisfcm.Naam}",
                        Instelling = $"{mgm.Waarde}",
                        TType = "TE_type"
                    });
                }
            }

            // Vooruit realisaties
            foreach (FaseCyclusModel fcm in controller.Fasen)
            {
                data.Elements.Add(new CCOLElement()
                {
                    Define = $"prmmlfpr{fcm.Naam}",
                    Naam = $"mlfpr{fcm.Naam}",
                    Instelling = "1",
                    TType = "0"
                });
            }

            // Alternatieven
            if (controller.ModuleMolen.LangstWachtendeAlternatief)
            {
                // alternatieve max. groentijd
                foreach (FaseCyclusModel fcm in controller.Fasen)
                {
                    data.Elements.Add(new CCOLElement()
                    {
                        Define = $"prmaltg{fcm.Naam}",
                        Naam = $"altg{fcm.Naam}",
                        Instelling = "60",
                        TType = "TE_type"
                    });
                }
                // alternatieve realisatieruimte
                foreach (FaseCyclusModel fcm in controller.Fasen)
                {
                    data.Elements.Add(new CCOLElement()
                    {
                        Define = $"prmaltp{fcm.Naam}",
                        Naam = $"altp{fcm.Naam}",
                        Instelling = "60",
                        TType = "TE_type"
                    });
                }
            }

            // Add last, nameless element for maximum #define
            data.Elements.Add(new CCOLElement() { Define = "PRMMAX" });


            data.SetMax();
            return data;
        }

        #endregion // Static Private Methods

    }
}
