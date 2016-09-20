using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Interfaces.Public
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class TLCGenGeneratorAttribute : System.Attribute
    {
        public TLCGenGeneratorAttribute()
        {
        }
    }
}
