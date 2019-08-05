using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    [Serializable]
    public class HalfstarFaseCyclusInstellingenModel : IComparable
    {
        [RefersTo(Enumerations.TLCGenObjectTypeEnum.Fase)]
        public string FaseCyclus { get; set; }

        public bool AlternatiefToestaan { get; set; }
        public int AlternatieveRuimte { get; set; }
        public bool AanvraagOpTxB { get; set; }
        public bool PrivilegePeriodeOpzetten { get; set; }

        public int CompareTo(object obj)
        {
            return FaseCyclus.CompareTo(((HalfstarFaseCyclusInstellingenModel)obj).FaseCyclus);
        }
    }
}
