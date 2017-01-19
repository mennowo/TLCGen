using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public class CCOLIOElement
    {
        public string Naam { get; set; }
        public IOElementModel Element { get; set; }

        public CCOLIOElement(IOElementModel ioelem, string naam)
        {
            Naam = naam;
            Element = ioelem;
        }
    }
}
