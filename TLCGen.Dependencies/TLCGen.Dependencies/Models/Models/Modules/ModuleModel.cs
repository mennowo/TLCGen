using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TLCGen.Models
{
    [Serializable]
    public class ModuleModel : IHaveName
    {
        [ModelName(Enumerations.TLCGenObjectTypeEnum.Module)]
        public string Naam { get; set; }

        [XmlArrayItem(ElementName = "FaseCyclus")]
        public List<ModuleFaseCyclusModel> Fasen { get; set; }
        
        public ModuleModel()
        {
            Fasen = new List<ModuleFaseCyclusModel>();
        }
    }
}
