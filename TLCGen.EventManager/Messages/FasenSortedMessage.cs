using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.Messaging.Messages
{
    public class FasenSortedMessage
    {
        public List<FaseCyclusModel> Fasen { get; private set; }

        public FasenSortedMessage(List<FaseCyclusModel> fasenlist)
        {
            Fasen = fasenlist;
        }
    }
}
