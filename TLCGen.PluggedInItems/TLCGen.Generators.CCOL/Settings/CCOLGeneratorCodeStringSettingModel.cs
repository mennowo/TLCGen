using System;

namespace TLCGen.Generators.CCOL.Settings
{
    [Serializable]
    public class CCOLGeneratorCodeStringSettingModel
    {
        #region Fields

        private string _setting;

        #endregion // Fields

        #region Properties

        public string Default { get; set; }

        public string Setting
        {
            get => _setting;
            set => _setting = !string.IsNullOrWhiteSpace(value) ? value : Default;
        }
        
        public CCOLGeneratorSettingTypeEnum Type { get; set; }
        
        public string Description { get; set; }
        
        public string Categorie { get; set; }
        
        public string SubCategorie { get; set; }

        #endregion // Properties

        #region Public Methods

        public override string ToString()
        {
            return Setting == "_" ? "" : Setting;
        }

        #endregion // Public Methods
    }
}
