using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TLCGen.Helpers;
using TLCGen.Models;

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


    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum CCOLElementTypeEnum
    {
        TE_type,
        TS_type,
        TM_type
    }

    [Serializable]
    public class GebruikersOptieModel
    {
        public string Naam { get; set; }

        public CCOLElementTypeEnum Type { get; set; }
        public int? Instelling { get; set; }
        public string Commentaar { get; set; }

        public bool ShouldSerializeType()
        {
            return Instelling.HasValue;
        }
        public bool ShouldSerializeInstelling()
        {
            return Instelling.HasValue;
        }
        public bool ShouldSerializeCommentaar()
        {
            return !string.IsNullOrWhiteSpace(Commentaar);
        }
    }

    [Serializable]
    public class GebruikersOptieWithIOModel : IOElementModel
    {
        public override string Naam { get; set; }
        public string Commentaar { get; set; }
        
        public bool ShouldSerializeCommentaar()
        {
            return !string.IsNullOrWhiteSpace(Commentaar);
        }
    }
}
