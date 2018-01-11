using GalaSoft.MvvmLight.Messaging;
using TLCGen.Models;

namespace TLCGen.Messaging.Messages
{
    public class FileIngreepTeDoserenSignaalGroepPercentageChangedMessage : MessageBase
    {
        public FileIngreepTeDoserenSignaalGroepModel TeDoserenSignaalGroep { get; }

        public FileIngreepTeDoserenSignaalGroepPercentageChangedMessage(FileIngreepTeDoserenSignaalGroepModel fileingreep)
        {
            TeDoserenSignaalGroep = fileingreep;
        }
    }
}
