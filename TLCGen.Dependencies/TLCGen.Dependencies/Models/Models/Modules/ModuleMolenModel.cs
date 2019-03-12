using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TLCGen.Models
{
    [Serializable]
    public class ModuleMolenModel
    {
        [XmlArrayItem(ElementName = "Module")]
        public List<ModuleModel> Modules { get; set; }

        [XmlArrayItem(ElementName = "FaseCyclusModuleData")]
        public List<FaseCyclusModuleDataModel> FasenModuleData { get; set; } 

        [HasDefault(false)]
        public string WachtModule { get; set; }

        [HasDefault(false)]
        public string Reeks { get; set; }

        public bool LangstWachtendeAlternatief { get; set; }

        public ModuleMolenModel()
        {
            Modules = new List<ModuleModel>();
            FasenModuleData = new List<FaseCyclusModuleDataModel>();
        }
    }
}
