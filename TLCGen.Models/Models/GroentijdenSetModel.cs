using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class GroentijdenSetModel
    {
        #region Properties

        public string Naam { get; set; }
        public GroentijdenTypeEnum Type { get; set; }
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
