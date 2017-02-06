using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TLCGen.Models
{
    [Serializable]
    public class SignalenDataModel
    {
        [XmlArrayItem(ElementName = "WaarschuwingsGroep")]
        public List<WaarschuwingsGroepModel> WaarschuwingsGroepen { get; set; }

        [XmlArrayItem(ElementName = "Rateltikker")]
        public List<RatelTikkerModel> Rateltikkers { get; set; }

        public SignalenDataModel()
        {
            WaarschuwingsGroepen = new List<WaarschuwingsGroepModel>();
            Rateltikkers = new List<RatelTikkerModel>();
        }
    }
}
