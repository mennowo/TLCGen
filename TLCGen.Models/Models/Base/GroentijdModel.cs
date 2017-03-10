using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    [Serializable]
    [RefersToSignalGroup("FaseCyclus")]
    public class GroentijdModel : IComparable
    {
        #region Properties

        [Browsable(false)]
        public string FaseCyclus { get; set; }
        public int? Waarde { get; set; }

        #endregion // Properties

        #region IComparable

        public int CompareTo(object obj)
        {
            GroentijdModel m = obj as GroentijdModel;
            if (m == null)
                throw new NotImplementedException();

            return this.FaseCyclus.CompareTo(m.FaseCyclus);
        }

        #endregion // IComparable
    }
}
