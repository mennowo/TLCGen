using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL
{
    public partial class CCOLCodeGenerator
    {

        internal enum CCOLElementType { Uitgang, Ingang, HulpElement, GeheugenElement, Timer, Counter, Schakelaar, Parameter };
        internal class CCOLElement
        {
            public string Define { get; set; }
            public string Naam { get; set; }
            public string Type { get; set; }
            public string TType { get; set; }
            public string Instelling { get; set; }
        }

        internal class CCOLElemListData
        {
            public List<CCOLElement> Elements { get; set; }

            public string CCOLCode { get; set; }
            public string CCOLSetting { get; set; }
            public string CCOLTType{ get; set; }

            public int CCOLCodeWidth { get { return CCOLCode == null ? 0 : CCOLCode.Length; } }
            public int CCOLSettingWidth { get { return CCOLSetting == null ? 0 : CCOLSetting.Length; } }
            public int CCOLTTypeWidth { get { return CCOLTType == null ? 0 : CCOLTType.Length; } }

            public int DefineMaxWidth { get; set; }
            public int NameMaxWidth { get; set; }
            public int SettingMaxWidth { get; set; }

            public void SetMax()
            {
                foreach (CCOLElement elem in this.Elements)
                {
                    if (elem.Define?.Length > this.DefineMaxWidth) this.DefineMaxWidth = elem.Define.Length;
                    if (elem.Naam?.Length > this.NameMaxWidth) this.NameMaxWidth = elem.Naam.Length;
                    if (elem.Instelling?.Length > this.SettingMaxWidth) this.SettingMaxWidth = elem.Instelling.Length;
                }
            }

            public CCOLElemListData()
            {
                DefineMaxWidth = 0;
                NameMaxWidth = 0;
                SettingMaxWidth = 0;

                Elements = new List<CCOLElement>();
            }
        }

        private void CollectAllCCOLElements(ControllerModel controller)
        {
            Uitgangen = CollectAllUitgangen(controller);
            Ingangen = CollectAllIngangen(controller);
            HulpElementen = CollectAllHulpElementen(controller);
            GeheugenElementen = CollectAllGeheugenElementen(controller);
            Timers = CollectAllTimers(controller);
            Counters = CollectAllCounters(controller);
            Schakelaars = CollectAllSchakelaars(controller);
            Parameters = CollectAllParameters(controller);
        }

        private string GetAllElementsSysHLines(CCOLElemListData data, string defmax)
        {
            StringBuilder sb = new StringBuilder();

            int pad1 = data.DefineMaxWidth + $"{tabspace}#define  ".Length;
            int _pad1 = defmax.Length + $"{tabspace}#define  ".Length;
            if (_pad1 > pad1) pad1 = _pad1;
            int pad2 = data.Elements.Count.ToString().Length;

            int index = 0;
            foreach (CCOLElement elem in data.Elements)
            {
                sb.Append($"{tabspace}#define {elem.Define} ".PadRight(pad1));
                sb.AppendLine($"{index.ToString()}".PadLeft(pad2));
                ++index;
            }
            sb.Append($"{tabspace}#define {defmax} ".PadRight(pad1));
            sb.Append($"{index.ToString()}".PadLeft(pad2));
            sb.AppendLine(" /* max */");

            return sb.ToString();
        }

        private string GetAllElementsTabCLines(CCOLElemListData data)
        {
            StringBuilder sb = new StringBuilder();

            int pad1 = data.CCOLCodeWidth + 2 + data.DefineMaxWidth; // 3: [ ]
            int pad2 = data.NameMaxWidth + 6;  // 6: space = space " " ;
            int pad3 = data.CCOLSettingWidth + 3 + data.DefineMaxWidth; // 3: space [ ]
            int pad4 = data.SettingMaxWidth + 4;  // 4: space = space ;
            int pad5 = data.CCOLTTypeWidth + 3 + data.DefineMaxWidth; // 3: space [ ]

            foreach (CCOLElement ce in data.Elements)
            {
                sb.Append($"{tabspace}{data.CCOLCode}[{ce.Define}]".PadRight(pad1));
                sb.Append($" = \"{ce.Naam}\";".PadRight(pad2));
                if (!string.IsNullOrEmpty(data.CCOLSetting))
                {
                    sb.Append($" {data.CCOLSetting}[{ce.Define}]".PadRight(pad3));
                    sb.Append($" = {ce.Instelling.ToString()};".PadRight(pad4));
                }
                if (!string.IsNullOrEmpty(data.CCOLTType))
                {
                    sb.Append($" {data.CCOLTType}[{ce.Define}]".PadRight(pad5));
                    sb.Append($" = {ce.TType};");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private CCOLElemListData CollectAllUitgangen(ControllerModel controller)
        {
            CCOLElemListData data = new CCOLElemListData();

            data.CCOLCode = "US_code";

            data.SetMax();
            return data;
        }

        private CCOLElemListData CollectAllIngangen(ControllerModel controller)
        {
            CCOLElemListData data = new CCOLElemListData();

            data.CCOLCode = "IS_code";

            data.SetMax();
            return data;
        }

        private CCOLElemListData CollectAllHulpElementen(ControllerModel controller)
        {
            CCOLElemListData data = new CCOLElemListData();

            data.CCOLCode = "HE_code";

            data.SetMax();
            return data;
        }

        private CCOLElemListData CollectAllGeheugenElementen(ControllerModel controller)
        {
            CCOLElemListData data = new CCOLElemListData();

            data.CCOLCode = "MM_code";

            data.Elements.Add(new CCOLElement() { Define = "mperiod", Naam = "PERIOD" });

            data.SetMax();
            return data;
        }

        private CCOLElemListData CollectAllTimers(ControllerModel controller)
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

            data.SetMax();
            return data;
        }

        private CCOLElemListData CollectAllSchakelaars(ControllerModel controller)
        {
            CCOLElemListData data = new CCOLElemListData();

            data.CCOLCode = "SCH_code";
            data.CCOLSetting = "SCH";

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

            data.SetMax();
            return data;
        }

        private CCOLElemListData CollectAllCounters(ControllerModel controller)
        {
            CCOLElemListData data = new CCOLElemListData();

            data.CCOLCode = "C_code";
            data.CCOLSetting = "C_max";
            data.CCOLTType = "C_type";

            data.SetMax();
            return data;
        }

        private CCOLElemListData CollectAllParameters(ControllerModel controller)
        {
            CCOLElemListData data = new CCOLElemListData();

            data.CCOLCode = "PRM_code";
            data.CCOLSetting = "PRM";
            data.CCOLTType = "PRM_type";

            data.Elements.Add(new CCOLElement() { Define = "prmfb", Naam = "FB", Instelling = "240", TType = "TS_type" });

            data.SetMax();
            return data;
        }
    }
}
