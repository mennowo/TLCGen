using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TLCGen.Models
{
    [Serializable]
    public class RoBuGroverConflictGroepModel
    {
        [XmlArrayItem(ElementName = "RoBuGroverConflictGroepFase")]
        public List<RoBuGroverConflictGroepFaseModel> Fasen { get; set; }

        public RoBuGroverConflictGroepModel()
        {
            Fasen = new List<RoBuGroverConflictGroepFaseModel>();
        }
    }

}