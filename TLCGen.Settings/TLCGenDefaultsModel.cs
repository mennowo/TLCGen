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
    public class TLCGenDefaultsModel
    {
        [XmlArrayItem(ElementName = "FaseCyclus")]
        public List<FaseCyclusModel> Fasen { get; set; }
        [XmlArrayItem(ElementName = "Detector")]
        public List<DetectorModel> Detectoren { get; set; }

        public RoBuGroverConflictGroepFaseModel RoBuGroverFase { get; set; }
    }
}
