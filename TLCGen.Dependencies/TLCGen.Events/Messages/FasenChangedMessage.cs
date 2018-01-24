using System.Collections.Generic;

using TLCGen.Models;

namespace TLCGen.Messaging.Messages
{
    public class FasenChangingMessage
    {
        public List<FaseCyclusModel> AddedFasen { get; }
        public List<FaseCyclusModel> RemovedFasen { get; }

        public FasenChangingMessage(List<FaseCyclusModel> added, List<FaseCyclusModel> removed)
        {
            AddedFasen = added;
            RemovedFasen = removed;
        }
    }
}
