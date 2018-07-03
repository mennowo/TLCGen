namespace TLCGen.Generators.CCOL.CodeGeneration
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class CCOLCodePieceGeneratorAttribute : System.Attribute
    {
        /// <summary>
        /// Property Attribute to indicate a property is meant as setting for a
        /// class that implements ICCOLCodePieceGenerator.
        /// </summary>
        public CCOLCodePieceGeneratorAttribute()
        {
        }
    }
}
