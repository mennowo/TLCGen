using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    [Serializable]
    public class GeneratorDataModel
    {
        public string Naam { get; set; }
        public List<GeneratorDataPropertyModel> Properties { get; set; }

        public GeneratorDataModel()
        {
            Properties = new List<GeneratorDataPropertyModel>();
        }
    }
}
