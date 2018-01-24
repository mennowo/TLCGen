using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
