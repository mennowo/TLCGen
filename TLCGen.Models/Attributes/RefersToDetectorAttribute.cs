using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    public class RefersToDetectorAttribute : System.Attribute
    {
        public string DetectorProperty1 { get; set; }
        public string DetectorProperty2 { get; set; }

        public RefersToDetectorAttribute(string dprop1 = null, string dprop2 = null)
        {
            DetectorProperty1 = dprop1;
            DetectorProperty2 = dprop2;
        }
    }
}
