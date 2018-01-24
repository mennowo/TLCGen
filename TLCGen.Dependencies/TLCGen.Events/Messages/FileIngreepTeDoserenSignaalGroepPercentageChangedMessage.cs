
using TLCGen.Models;

namespace TLCGen.Messaging.Messages
{
    public class FileIngreepTeDoserenSignaalGroepPercentageChangedMessage
    {
        public FileIngreepTeDoserenSignaalGroepModel TeDoserenSignaalGroep { get; }

        public FileIngreepTeDoserenSignaalGroepPercentageChangedMessage(FileIngreepTeDoserenSignaalGroepModel fileingreep)
        {
            TeDoserenSignaalGroep = fileingreep;
        }
    }
}
