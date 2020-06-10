using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class ModuleModel : IHaveName
    {
        [RefersTo(TLCGenObjectTypeEnum.Module)]
        [ModelName(Enumerations.TLCGenObjectTypeEnum.Module)]
        public string Naam { get; set; }

        [Browsable(false)] [HasDefault(false)] public TLCGenObjectTypeEnum ObjectType => TLCGenObjectTypeEnum.Module;

        [XmlArrayItem(ElementName = "FaseCyclus")]
        public List<ModuleFaseCyclusModel> Fasen { get; set; }
        
        public ModuleModel()
        {
            Fasen = new List<ModuleFaseCyclusModel>();
        }
    }
}
