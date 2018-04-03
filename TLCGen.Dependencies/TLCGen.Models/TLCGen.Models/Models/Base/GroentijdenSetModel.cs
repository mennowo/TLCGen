using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class GroentijdenSetModel : IHaveName
    {
        #region Properties

        [ModelName]
        public string Naam { get; set; }
        public GroentijdenTypeEnum Type { get; set; }
        [XmlElement(ElementName = "Groentijd")]
        public List<GroentijdModel> Groentijden { get; set; }

        #endregion // Properties

        #region Constructor

        public GroentijdenSetModel()
        {
            Groentijden = new List<GroentijdModel>();
        }

        #endregion // Constructor
    }
}
