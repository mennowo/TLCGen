using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TLCGen.Settings
{
    [Serializable]
    public class TLCGenDefaultsModel
    {
        public string DefaultsSetName { get; set; }

        [XmlArrayItem(ElementName = "Default")]
        public List<TLCGenDefaultModel> Defaults { get; set; }

        public TLCGenDefaultsModel()
        {
            Defaults = new List<TLCGenDefaultModel>();
        }
    }
}
