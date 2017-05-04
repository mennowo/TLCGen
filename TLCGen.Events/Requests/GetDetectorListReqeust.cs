using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Messaging.Requests
{
    public class GetDetectorListReqeust<T, TResult>
    {
        public Func<T, TResult> Callback { get; private set; }

        public GetDetectorListReqeust(Func<T, TResult> callback)
        {
            Callback = callback;
        }
    }
}
