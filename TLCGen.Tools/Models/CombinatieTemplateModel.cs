using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Plugins.Tools
{
    public class CombinatieTemplateModel
    {
        #region Properties

        public string Name { get; set; }
        public List<CombinatieTemplateItemModel> Items { get; set; }
        public List<CombinatieTemplateOptieModel> Opties { get; set; }

        #endregion // Properties

        #region Constructor

        public CombinatieTemplateModel()
        {
            Items = new List<CombinatieTemplateItemModel>();
            Opties = new List<CombinatieTemplateOptieModel>();
        }

        #endregion // Constructor
    }
}
