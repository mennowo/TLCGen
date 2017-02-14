using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TLCGen.Settings
{
    [Serializable]
    public class TLCGenTemplatesModel
    {
        [XmlArrayItem(ElementName = "FasenTemplate")]
        public List<FaseCyclusTemplateModel> FasenTemplates { get; set; }
        [XmlArrayItem(ElementName = "DetectorenTemplate")]
        public List<DetectorTemplateModel> DetectorenTemplates { get; set; }

        public TLCGenTemplatesModel()
        {
            FasenTemplates = new List<FaseCyclusTemplateModel>();
            DetectorenTemplates = new List<DetectorTemplateModel>();
        }
    }
}
