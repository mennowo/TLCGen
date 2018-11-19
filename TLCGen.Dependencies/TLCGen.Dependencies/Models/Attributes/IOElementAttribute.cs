using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Property)]
    public class IOElementAttribute : Attribute
    {
        public string DisplayName { get; private set; }
        public BitmappedItemTypeEnum Type { get; private set; }
        public string DisplayNameProperty { get; private set; }
        public string DisplayConditionProperty { get; private set; }

        public IOElementAttribute(string name, BitmappedItemTypeEnum type, string nameprop = null, string conditionprop = null)
        {
            DisplayName = name;
            Type = type;
            DisplayNameProperty = nameprop;
            DisplayConditionProperty = conditionprop;
        }
    }
}
