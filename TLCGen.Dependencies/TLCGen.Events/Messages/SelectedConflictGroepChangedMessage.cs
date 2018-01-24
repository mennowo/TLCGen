using GalaSoft.MvvmLight.Messaging;
using TLCGen.Models;

namespace TLCGen.Messaging.Messages
{
    public class SelectedConflictGroepChangedMessage : MessageBase
    {
        public RoBuGroverConflictGroepModel OldGroep { get; }
        public RoBuGroverConflictGroepModel NewGroep { get; }
        public bool NewGroupCheckConflicts { get; }

        public SelectedConflictGroepChangedMessage(RoBuGroverConflictGroepModel newgroep, RoBuGroverConflictGroepModel oldgroep, bool newgroupcheckconflicts)
        {
            OldGroep = oldgroep;
            NewGroep = newgroep;
            NewGroupCheckConflicts = newgroupcheckconflicts;
        }
    }
}
