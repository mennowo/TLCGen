using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TLCGen.Messaging
{
    public class Subscriber
    {
        public object MySubscriber { get; private set; }
        public WeakAction MyAction { get; private set; }

        public Subscriber(object subscriber, WeakAction action)
        {
            MySubscriber = subscriber;
            MyAction = action;
        }
    }
}
