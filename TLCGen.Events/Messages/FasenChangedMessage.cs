using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.Messaging.Messages
{
    public class FasenChangingMessage
    {
        public List<FaseCyclusModel> AddedFasen { get; private set; }
        public List<FaseCyclusModel> RemovedFasen { get; private set; }

        public FasenChangingMessage(List<FaseCyclusModel> added, List<FaseCyclusModel> removed)
        {
            AddedFasen = added;
            RemovedFasen = removed;
        }
    }
}
