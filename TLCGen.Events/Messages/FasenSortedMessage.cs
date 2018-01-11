using System.Collections.Generic;
using GalaSoft.MvvmLight.Messaging;
using TLCGen.Models;

namespace TLCGen.Messaging.Messages
{
    public class FasenSortedMessage : MessageBase
    {
        public List<FaseCyclusModel> Fasen { get; }

        public FasenSortedMessage(List<FaseCyclusModel> fasenlist)
        {
            Fasen = fasenlist;
        }
    }
}
