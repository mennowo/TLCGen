using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    [Serializable]
    public class GroentijdModel : IComparable
    {
        public string FaseCyclus { get; set; }
        public int? Waarde { get; set; }

        public int CompareTo(object obj)
        {
            GroentijdModel m = obj as GroentijdModel;
            if (m == null)
                throw new NotImplementedException();

            return this.FaseCyclus.CompareTo(m.FaseCyclus);
        }
    }
}
