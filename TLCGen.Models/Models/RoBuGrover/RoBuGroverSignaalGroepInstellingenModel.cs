using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TLCGen.Models
{
    [Serializable]
    public class RoBuGroverSignaalGroepInstellingenModel
    {
        public string FaseCyclus { get; set; }
        public int MinGroenTijd { get; set; }
        public int MaxGroenTijd { get; set; }

        [XmlArrayItem(ElementName = "RoBuGroverFileDetector")]
        public List<RoBuGroverFileDetectorModel> FileDetectoren { get; set; }
        [XmlArrayItem(ElementName = "RoBuGroverHiaatDetector")]
        public List<RoBuGroverHiaatDetectorModel> HiaatDetectoren { get; set; }

        public RoBuGroverSignaalGroepInstellingenModel()
        {
            FileDetectoren = new List<RoBuGroverFileDetectorModel>();
            HiaatDetectoren = new List<RoBuGroverHiaatDetectorModel>();
        }
    }
}