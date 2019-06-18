using System;

namespace TLCGen.Models
{
    /// <summary>
    /// This is meant to indicate a given property is meant to be left alone when
    /// renaming elements, indexing items for the bitmap, etc.
    /// </summary>
    [AttributeUsage(validOn: AttributeTargets.Property)]
    public class TLCGenIgnoreAttributeAttribute : Attribute
    {
        public TLCGenIgnoreAttributeAttribute()
        {
        }
    }
}
