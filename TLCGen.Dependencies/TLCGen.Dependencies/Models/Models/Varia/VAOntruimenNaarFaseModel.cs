using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class VAOntruimenNaarFaseModel : IComparable
    {
        #region Properties

        [RefersTo(TLCGenObjectTypeEnum.Fase)]
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