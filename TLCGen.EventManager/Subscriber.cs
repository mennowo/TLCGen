using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TLCGen.Messaging
{
    public class Subscriber
    {
        public object MySubscriber { get; set; }
        public WeakAction MyAction { get; set; }
    }
}
