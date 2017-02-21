using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Property)]
    public class IOElementAttribute : Attribute
    {
        public string DisplayName { get; set; }
        public BitmappedItemTypeEnum Type { get; set; }
        public string DisplayNameProperty { get; set; }
        public string DisplayConditionProperty { get; set; }

        public IOElementAttribute(string name, BitmappedItemTypeEnum type, string nameprop = null, string conditionprop = null)
        {
            DisplayName = name;
            Type = type;
            DisplayNameProperty = nameprop;
            DisplayConditionProperty = conditionprop;
        }
    }
}
