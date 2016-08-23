using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.Models
{
    public static class IDProvider
    {
        public static ControllerModel Controller { get; set; }

        public static long GetNextID()
        {
            if (Controller != null)
            {
                Controller.NextID++;
                return Controller.NextID;
            }
            else
            {
                return 0;
            }
        }
    }
}
