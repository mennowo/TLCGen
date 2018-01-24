using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TLCGen.Models
{
    [Serializable]
    public class CustomDataModel
    {
        [XmlArrayItem(ElementName = "CustomData")]
        public List<AddinSettingsModel> AddinSettings { get; set; }

        public CustomDataModel()
        {
            AddinSettings = new List<AddinSettingsModel>();
        }
    }
}
