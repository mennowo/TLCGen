using System;

namespace TLCGen.Models
{
    public class HasDefaultAttribute : Attribute
    {
        public bool HasDefault { get; set; }

        public HasDefaultAttribute(bool hasdefault)
        {
            HasDefault = hasdefault;
        }
    }
}
