using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    [Serializable]
    public class RoBuGroverConflictGroepModel
    {
        public List<RoBuGroverConflictGroepFaseModel> Fasen { get; set; }

        public RoBuGroverConflictGroepModel()
        {
            Fasen = new List<RoBuGroverConflictGroepFaseModel>();
        }
    }

}