using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.Messaging.Messages
{
    public class FileIngreepTeDoserenSignaalGroepPercentageChangedMessage
    {
        public FileIngreepTeDoserenSignaalGroepModel TeDoserenSignaalGroep { get; private set; }

        public FileIngreepTeDoserenSignaalGroepPercentageChangedMessage(FileIngreepTeDoserenSignaalGroepModel fileingreep)
        {
            TeDoserenSignaalGroep = fileingreep;
        }
    }
}
