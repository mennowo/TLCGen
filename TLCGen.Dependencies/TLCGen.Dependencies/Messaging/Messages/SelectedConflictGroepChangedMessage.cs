
using TLCGen.Models;

namespace TLCGen.Messaging.Messages
{
    public class SelectedConflictGroepChangedMessage
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
