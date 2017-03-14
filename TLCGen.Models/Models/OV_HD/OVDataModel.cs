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

        [Browsable(false)]
        [XmlIgnore]
        public bool HasKAR
        {
            get
            {
                return this.OVIngrepen.Count > 0 && this.OVIngrepen.Where(x => x.KAR).Any() ||
                       this.HDIngrepen.Count > 0 && this.HDIngrepen.Where(x => x.KAR).Any();
            }
        }

        [Browsable(false)]
        [IOElement("karmelding", BitmappedItemTypeEnum.Uitgang, null, "HasKAR")]
        public BitmapCoordinatenDataModel KARMeldingBitmapData { get; set; }

        [Browsable(false)]
        [IOElement("karog", BitmappedItemTypeEnum.Uitgang, null, "HasKAR")]
        public BitmapCoordinatenDataModel KAROnderGedragBitmapData { get; set; }

        [XmlElement(ElementName = "OVIngreep")]
        public List<OVIngreepModel> OVIngrepen { get; set; }
        [XmlElement(ElementName = "HDIngreep")]
        public List<HDIngreepModel> HDIngrepen { get; set; }
        public List<OVIngreepSignaalGroepParametersModel> OVIngreepSignaalGroepParameters { get; set; }

        public OVDataModel()
        {
            KARMeldingBitmapData = new BitmapCoordinatenDataModel();
            KAROnderGedragBitmapData = new BitmapCoordinatenDataModel();
            OVIngrepen = new List<OVIngreepModel>();
            HDIngrepen = new List<HDIngreepModel>();
            OVIngreepSignaalGroepParameters = new List<OVIngreepSignaalGroepParametersModel>();
        }
    }
}
