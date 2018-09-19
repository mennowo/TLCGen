using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TLCGen.Plugins.AFM.Models
{

    [Serializable]
    [XmlRoot(ElementName = "AFMData")]
    public class AFMDataModel
    {
        public bool AFMToepassen { get; set; }

        [XmlArray(ElementName = "AFMFaseCyclusData")]
        public List<AFMFaseCyclusDataModel> AFMFasen { get; set; }

        public AFMDataModel()
        {
            AFMFasen = new List<AFMFaseCyclusDataModel>();
        }
    }
}
