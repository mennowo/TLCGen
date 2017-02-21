using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Messaging.Requests
{
    public class ProcessSynchronisationsRequest
    {
        public bool Succes { get; set; }

        public ProcessSynchronisationsRequest()
        {
            Succes = false;
        }
    }
}
