using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    public class RefersToSignalGroupAttribute : System.Attribute
    {
        public string SignalGroupProperty1 { get; set; }
        public string SignalGroupProperty2 { get; set; }

        public RefersToSignalGroupAttribute(string sgprop1 = null, string sgprop2 = null)
        {
            SignalGroupProperty1 = sgprop1;
            SignalGroupProperty2 = sgprop2;
        }
    }
}
