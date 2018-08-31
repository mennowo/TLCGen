namespace TLCGen.Models
{
    public class RefersToAttribute : System.Attribute
    {
        public string ReferProperty1 { get; }
        public string ReferProperty2 { get; }
        public string ReferProperty3 { get; }

        public RefersToAttribute(string refprop1 = null, string refprop2 = null, string refprop3 = null)
        {
	        ReferProperty1 = refprop1;
	        ReferProperty2 = refprop2;
	        ReferProperty3 = refprop3;
        }
    }
}
