using System.Collections.Generic;

using TLCGen.Models;

namespace TLCGen.Messaging.Messages
{
    public class FasenChangedMessage
    {
        public List<FaseCyclusModel> AddedFasen { get; }
        public List<FaseCyclusModel> RemovedFasen { get; }

        public FasenChangedMessage(List<FaseCyclusModel> added, List<FaseCyclusModel> removed)
        {
            AddedFasen = added;
            RemovedFasen = removed;
        }
    }
}
