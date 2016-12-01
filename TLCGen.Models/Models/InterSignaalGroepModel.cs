using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TLCGen.Models
{
    [Serializable]
    public class InterSignaalGroepModel
    {
        [XmlArrayItem(ElementName = "Conflict")]
        public List<ConflictModel> Conflicten { get; set; }

        [XmlArrayItem(ElementName = "Voorstart")]
        public List<VoorstartModel> Voorstarten { get; set; }

        [XmlArrayItem(ElementName = "Gelijkstart")]
        public List<GelijkstartModel> Gelijkstarten { get; set; }

        [XmlArrayItem(ElementName = "Naloop")]
        public List<NaloopModel> Nalopen { get; set; }

        [XmlArrayItem(ElementName = "Meeaanvraag")]
        public List<MeeaanvraagModel> Meeaanvragen { get; set; }

        public InterSignaalGroepModel()
        {
            Conflicten = new List<ConflictModel>();
            Voorstarten = new List<VoorstartModel>();
            Gelijkstarten = new List<GelijkstartModel>();
            Nalopen = new List<NaloopModel>();
            Meeaanvragen = new List<MeeaanvraagModel>();
        }
    }
}
