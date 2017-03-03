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
    public class OVDataModel
    {
        public OVIngreepTypeEnum OVIngreepType { get; set; }
        public bool DSI { get; set; }
        public bool CheckOpDSIN { get; set; }

        [XmlElement(ElementName = "OVIngreep")]
        public List<OVIngreepModel> OVIngrepen { get; set; }
        [XmlElement(ElementName = "HDIngreep")]
        public List<HDIngreepModel> HDIngrepen { get; set; }
        public List<OVIngreepSignaalGroepParametersModel> OVIngreepSignaalGroepParameters { get; set; }

        public OVDataModel()
        {
            OVIngrepen = new List<OVIngreepModel>();
            HDIngrepen = new List<HDIngreepModel>();
            OVIngreepSignaalGroepParameters = new List<OVIngreepSignaalGroepParametersModel>();
        }
    }
}
