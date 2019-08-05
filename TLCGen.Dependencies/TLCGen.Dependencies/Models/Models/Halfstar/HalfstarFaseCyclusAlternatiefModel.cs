using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    [Serializable]
    public class HalfstarFaseCyclusAlternatiefModel
    {
        [RefersTo(Enumerations.TLCGenObjectTypeEnum.Fase)]
        public string FaseCyclus { get; set; }

        public bool AlternatiefToestaan { get; set; }
        public int AlternatieveRuimte { get; set; }
    }
}
