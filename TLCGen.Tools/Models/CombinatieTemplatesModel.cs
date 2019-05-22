using System.Collections.Generic;

namespace TLCGen.Plugins.Tools
{
    public class CombinatieTemplatesModel
    {
        #region Properties

        public List<CombinatieTemplateModel> CombinatieTemplates { get; set; }

        #endregion // Properties

        #region Constructor

        public CombinatieTemplatesModel()
        {
            CombinatieTemplates = new List<CombinatieTemplateModel>();
        }

        #endregion // Constructor
    }
}
