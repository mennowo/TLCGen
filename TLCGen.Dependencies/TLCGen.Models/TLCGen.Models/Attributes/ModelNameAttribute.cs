using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ModelNameAttribute : Attribute
    {
        public TLCGenObjectTypeEnum Type { get; }

        public ModelNameAttribute(TLCGenObjectTypeEnum type)
        {
            Type = type;
        }
    }
}
