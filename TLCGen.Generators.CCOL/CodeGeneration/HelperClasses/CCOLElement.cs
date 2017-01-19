using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public class CCOLElement
    {
        public string Define { get; set; }
        public string Naam { get; set; }
        public CCOLElementType Type { get; set; }
        public string TType { get; set; }
        public string Instelling { get; set; }
        public string Commentaar { get; set; }
    }
}
