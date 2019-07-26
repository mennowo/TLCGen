using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public class CCOLKoppelSignaal
    {
        public int Order { get; set; }
        public int Count { get; set; }
        public string Name { get; set; }
        public string Koppeling { get; set; }
        public KoppelSignaalRichtingEnum Richting { get; set; }
    }
}
