using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    public class RefersToAttribute : System.Attribute
    {
        public string ReferProperty1 { get; }
        public string ReferProperty2 { get; }

        public RefersToAttribute(string refprop1, string refprop2 = null)
        {
	        ReferProperty1 = refprop1;
	        ReferProperty2 = refprop2;
		}
    }
}
