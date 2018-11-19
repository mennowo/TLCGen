using System.Collections.Generic;

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
