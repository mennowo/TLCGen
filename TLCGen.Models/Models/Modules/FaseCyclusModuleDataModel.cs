using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    [Serializable]
    public class FaseCyclusModuleDataModel
    {
        public string FaseCyclus { get; set; }
        public int ModulenVooruit { get; set; }
        public bool AlternatiefToestaan { get; set; }
        public int AlternatieveRuimte { get; set; }
        public int AlternatieveGroenTijd { get; set; }
    }
}
