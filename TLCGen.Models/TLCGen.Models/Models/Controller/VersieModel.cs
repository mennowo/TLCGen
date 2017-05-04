using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    [Serializable]
    public class VersieModel
    {
        public string Versie { get; set; }
        public DateTime Datum { get; set; }
        public string Ontwerper { get; set; }
        public string Commentaar { get; set; }

        public VersieModel()
        {
            Datum = new DateTime();
        }
    }
}
