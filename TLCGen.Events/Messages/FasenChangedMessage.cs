using System.Collections.Generic;
using GalaSoft.MvvmLight.Messaging;
using TLCGen.Models;

namespace TLCGen.Messaging.Messages
{
    public class FasenChangingMessage : MessageBase
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
