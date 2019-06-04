using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TLCGen.GebruikersOpties
{
    [Serializable]
    [XmlRoot(ElementName = "GebruikersOpties")]
    public class GebruikersOptiesModel
    {
        [XmlArrayItem(ElementName = "Uitgang")]
        public List<GebruikersOptieWithIOModel> Uitgangen { get; set; }
        [XmlArrayItem(ElementName = "Ingang")]
        public List<GebruikersOptieWithIOModel> Ingangen { get; set; }
        [XmlArrayItem(ElementName = "HulpElement")]
        public List<GebruikersOptieModel> HulpElementen { get; set; }
        [XmlArrayItem(ElementName = "Timer")]
        public List<GebruikersOptieModel> Timers { get; set; }
        [XmlArrayItem(ElementName = "Counter")]
        public List<GebruikersOptieModel> Counters { get; set; }
        [XmlArrayItem(ElementName = "Schakelaar")]
        public List<GebruikersOptieModel> Schakelaars { get; set; }
        [XmlArrayItem(ElementName = "GeheugenElement")]
        public List<GebruikersOptieModel> GeheugenElementen { get; set; }
        [XmlArrayItem(ElementName = "Parameter")]
        public List<GebruikersOptieModel> Parameters { get; set; }

        public GebruikersOptiesModel()
        {
            Uitgangen = new List<GebruikersOptieWithIOModel>();
            Ingangen = new List<GebruikersOptieWithIOModel>();
            HulpElementen = new List<GebruikersOptieModel>();
            Timers = new List<GebruikersOptieModel>();
            Counters = new List<GebruikersOptieModel>();
            Schakelaars = new List<GebruikersOptieModel>();
            GeheugenElementen = new List<GebruikersOptieModel>();
            Parameters = new List<GebruikersOptieModel>();
        }
    }
}
