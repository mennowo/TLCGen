using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.Messaging.Requests
{
    public class PrepareForGenerationRequest
    {
        public ControllerModel Controller { get; }
        public PrepareForGenerationRequest(ControllerModel controller)
        {
            Controller = controller;
        }
    }
}
