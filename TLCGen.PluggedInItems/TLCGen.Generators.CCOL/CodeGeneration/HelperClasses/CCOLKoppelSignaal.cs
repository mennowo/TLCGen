using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public enum CCOLKoppelSignaalRichtingEnum { In, Uit };

    public class CCOLKoppelSignaal
    {
        public int Order { get; set; }
        public int Count { get; set; }
        public string Name { get; set; }
        public CCOLKoppelSignaalRichtingEnum Richting { get; set; }
    }
}
