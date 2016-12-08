using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    [Serializable]
    public class OVDataModel
    {
        public List<OVIngreepModel> OVIngrepen { get; set; }
        public List<HDIngreepModel> HDIngrepen { get; set; }
        public List<OVSignaalGroepParametersModel> OVSignaalGroepParameters { get; set; }

        public OVDataModel()
        {
            OVIngrepen = new List<OVIngreepModel>();
            HDIngrepen = new List<HDIngreepModel>();
            OVSignaalGroepParameters = new List<OVSignaalGroepParametersModel>();
        }
    }
}
