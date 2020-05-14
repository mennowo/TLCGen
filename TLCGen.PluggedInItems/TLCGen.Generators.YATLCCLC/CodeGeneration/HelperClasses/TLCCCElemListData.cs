using System.Collections.Generic;

namespace TLCGen.Generators.TLCCC.CodeGeneration.HelperClasses
{
    public class TLCCCElemListData
    {
        public List<TLCCCElement> Elements { get; set; }

        public string CCOLCode { get; set; }
        public string CCOLSetting { get; set; }
        public string CCOLTType { get; set; }

        public int CCOLCodeWidth => CCOLCode == null ? 0 : CCOLCode.Length;
        public int CCOLSettingWidth => CCOLSetting == null ? 0 : CCOLSetting.Length;
        public int CCOLTTypeWidth => CCOLTType == null ? 0 : CCOLTType.Length;

        public int DefineMaxWidth { get; set; }
        public int NameMaxWidth { get; set; }
        public int SettingMaxWidth { get; set; }

        public void SetMax()
        {
            foreach (var elem in Elements)
            {
                if (elem.Define?.Length > DefineMaxWidth) DefineMaxWidth = elem.Define.Length;
                if (elem.Naam?.Length > NameMaxWidth) NameMaxWidth = elem.Naam.Length;
                if (elem.Instelling?.ToString().Length > SettingMaxWidth) SettingMaxWidth = elem.Instelling.ToString().Length;
            }
        }

        public TLCCCElemListData()
        {
            DefineMaxWidth = 0;
            NameMaxWidth = 0;
            SettingMaxWidth = 0;

            Elements = new List<TLCCCElement>();
        }
    }
}