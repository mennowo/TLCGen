using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TLCGen.Models
{
    [Serializable]
    public class VAOntruimenFaseModel
    {
        public string FaseCyclus { get; set; }
        public int MaximaleVAOntruimingsTijd { get; set; }

        [XmlArrayItem(ElementName = "ConflictendeFase")]
        public List<VAOntruimenNaarFaseModel> ConflicterendeFasen { get; set; }

        public VAOntruimenFaseModel()
        {
            ConflicterendeFasen = new List<VAOntruimenNaarFaseModel>();
        }
    }
}
