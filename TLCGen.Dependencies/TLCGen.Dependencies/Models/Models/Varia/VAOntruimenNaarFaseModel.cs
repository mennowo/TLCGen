using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    [Serializable]
    [RefersTo("FaseCyclus")]
    public class VAOntruimenNaarFaseModel : IComparable
    {
        #region Properties

        [HasDefault(false)]
        public string FaseCyclus { get; set; }
        public int VAOntruimingsTijd { get; set; }

        #endregion //Properties

        #region IComparable

        public int CompareTo(object obj)
        {
            var other = obj as VAOntruimenNaarFaseModel;
            if(obj != null)
            {
                return this.FaseCyclus.CompareTo(other.FaseCyclus);
            }
            else
            {
                throw new InvalidCastException($"Cannot cast type {obj.GetType()} to {this.GetType()}");
            }
        }

        #endregion // IComparable
    }
}