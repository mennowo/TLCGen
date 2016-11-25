using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models.Settings
{
    [Serializable]
    public class ApplicationSettings
    {
        public bool GarantieOntruimingsTijden { get; set; }
        public GroentijdenTypeEnum TypeGroentijden { get; set; }

        public ApplicationSettings()
        {
        }
    }
}