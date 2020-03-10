

using System.Collections.Generic;
using System.Windows.Documents;
using TLCGen.Models;

namespace TLCGen.Messaging.Messages
{
	public class IngangenChangedMessage
    {
        public List<IngangModel> RemovedIngangen { get; }
        public List<IngangModel> AddedIngangen { get; }

        public IngangenChangedMessage(List<IngangModel> removedIngangen, List<IngangModel> addedIngangen)
        {
            RemovedIngangen = removedIngangen;
            AddedIngangen = addedIngangen;
        }
    }
}
