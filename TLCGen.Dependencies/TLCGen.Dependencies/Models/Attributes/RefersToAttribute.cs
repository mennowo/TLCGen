using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RefersToAttribute : Attribute
    {
        public TLCGenObjectTypeEnum ObjectType { get; }
        public string ObjectTypeProperty { get; }

        public RefersToAttribute(TLCGenObjectTypeEnum objectType, string objectTypeProperty = null)
        {
            ObjectType = objectType;
            ObjectTypeProperty = objectTypeProperty;
        }
    }
}
