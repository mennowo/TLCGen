using System;

namespace TLCGen.Controls
{
    public class BrowsableConditionAttribute : Attribute
    {
        public BrowsableConditionAttribute(string conditionPropertyName)
        {
            ConditionPropertyName = conditionPropertyName;
        }

        public string ConditionPropertyName { get; }
    }
}
