using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public class CCOLIOElement
    {
        public string Naam { get; set; }
        public IOElementModel Element { get; set; }
        public bool Dummy { get; set; }

        public CCOLIOElement(IOElementModel ioelem, string naam)
        {
            Naam = naam;
            Element = ioelem;
            Dummy = false;
        }
    }
}
