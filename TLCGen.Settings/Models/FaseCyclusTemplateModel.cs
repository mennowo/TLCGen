using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TLCGen.Models;

namespace TLCGen.Settings
{
    [Serializable]
    public class FaseCyclusTemplateModel : TemplateModelBase
    {
        [XmlArrayItem(ElementName = "Fase")]
        public List<FaseCyclusModel> Fasen { get; set; }

        public FaseCyclusTemplateModel()
        {
            Fasen = new List<FaseCyclusModel>();
        }

        public override List<object> GetItems()
        {
            List<object> items = new List<object>();
            foreach(var fc in Fasen)
            {
                items.Add(fc);
            }
            return items;
        }
    }
}
