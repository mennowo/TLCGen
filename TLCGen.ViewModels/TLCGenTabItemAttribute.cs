namespace TLCGen.ViewModels
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class TLCGenTabItemAttribute : System.Attribute
    {
        /// <summary>
        /// Zero based index indicating how to order tabs loaded into TLCGen
        /// </summary>
        public int Index { get; set; }

        public TLCGenTabItemAttribute(int index)
        {
            Index = index;
        }
    }
}
