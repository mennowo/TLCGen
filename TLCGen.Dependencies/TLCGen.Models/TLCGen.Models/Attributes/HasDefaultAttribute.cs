using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
