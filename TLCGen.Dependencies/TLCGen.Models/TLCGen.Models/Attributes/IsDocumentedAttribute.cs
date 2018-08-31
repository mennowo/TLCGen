using System;

namespace TLCGen.Models
{
    public class IsDocumentedAttribute : Attribute
    {
        public string ConditionProperty { get; private set; }
        public string ConditionPropertyValue { get; private set; }
        public string ValueMustBe { get; private set; }
        public string ValueMustNotBe { get; private set; }

        public IsDocumentedAttribute(string conditionProperty = null, string conditionPropertyValue = null, string valueMustBe = null, string valueMustNotBe = null)
        {
            ConditionPropertyValue = conditionPropertyValue;
            ConditionProperty = conditionProperty;
            ValueMustBe = valueMustBe;
            ValueMustNotBe = valueMustNotBe;
        }
    }
}
