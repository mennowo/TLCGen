using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class RoBuGroverModel
    {
        public int MaximaleCyclustijd { get; set; }
        public RoBuGroverMethodeEnum MethodeRoBuGrover { get; set; }
        public bool OphogenTijdensGroen { get; set; }

        public List<RoBuGroverConflictGroepModel> ConflictGroepen { get; set; }
        public List<RoBuGroverSignaalGroepInstellingenModel> SignaalGroepInstellingen { get; set; }

        public RoBuGroverModel()
        {
            ConflictGroepen = new List<RoBuGroverConflictGroepModel>();
            SignaalGroepInstellingen = new List<RoBuGroverSignaalGroepInstellingenModel>();
        }
    }
}
