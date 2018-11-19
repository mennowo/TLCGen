using System.Collections.Generic;

using TLCGen.Models;

namespace TLCGen.Messaging.Messages
{
    public class FasenSortedMessage
    {
        public List<FaseCyclusModel> Fasen { get; }

        public FasenSortedMessage(List<FaseCyclusModel> fasenlist)
        {
            Fasen = fasenlist;
        }
    }
}
