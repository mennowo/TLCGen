using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    public class MaxGroentijdenSetModel
    {
        #region Properties

        public string Naam { get; set; }
        public List<MaxGroentijdModel> MaxGroentijden { get; set; }

        #endregion // Properties

        #region Constructor

        public MaxGroentijdenSetModel()
        {
            MaxGroentijden = new List<MaxGroentijdModel>();
        }

        #endregion // Constructor
    }
}
