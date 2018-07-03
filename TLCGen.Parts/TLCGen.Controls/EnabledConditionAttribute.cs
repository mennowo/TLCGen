using System;

namespace TLCGen.Controls
{
    public class EnabledConditionAttribute : Attribute
    {
        public EnabledConditionAttribute(string conditionPropertyName)
        {
            ConditionPropertyName = conditionPropertyName;
        }

        public string ConditionPropertyName { get; }
    }
}
