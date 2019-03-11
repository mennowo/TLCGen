using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    public class RefersToAttribute : System.Attribute
    {
        public TLCGenObjectTypeEnum ObjectType1 { get; }
        public TLCGenObjectTypeEnum ObjectType2 { get; }
        public TLCGenObjectTypeEnum ObjectType3 { get; }
        public string ReferProperty1 { get; }
        public string ReferProperty2 { get; }
        public string ReferProperty3 { get; }

        public RefersToAttribute(TLCGenObjectTypeEnum objectType1, string refprop1 = null, TLCGenObjectTypeEnum objectType2 = TLCGenObjectTypeEnum.Fase, string refprop2 = null, TLCGenObjectTypeEnum objectType3 = TLCGenObjectTypeEnum.Fase, string refprop3 = null)
        {
            ObjectType1 = objectType1;
            ObjectType2 = objectType2;
            ObjectType3 = objectType3;
            ReferProperty1 = refprop1;
	        ReferProperty2 = refprop2;
	        ReferProperty3 = refprop3;
        }
    }
}
