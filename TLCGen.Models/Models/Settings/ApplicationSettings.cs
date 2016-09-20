using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models.Settings
{
    [Serializable]
    public class ApplicationSettings
    {
        public bool GarantieOntruimingsTijden { get; set; }

        public ApplicationSettings()
        {
        }
    }
}