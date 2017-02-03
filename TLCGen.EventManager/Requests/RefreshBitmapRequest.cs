using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models.Enumerations;

namespace TLCGen.Messaging.Requests
{
    public class RefreshBitmapRequest
    {
        public List<System.Drawing.Point> Coordinates { get; private set; }
        
        public RefreshBitmapRequest(List<System.Drawing.Point> coordinates = null)
        {
            Coordinates = coordinates;
        }
    }
}
