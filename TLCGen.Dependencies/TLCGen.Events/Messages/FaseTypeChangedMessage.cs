using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Messaging.Messages
{
    public class FaseTypeChangedMessage
    {
        public FaseCyclusModel Fase { get; }
        public FaseTypeEnum OldType { get; }
        public FaseTypeEnum NewType { get; }

        public FaseTypeChangedMessage(FaseCyclusModel fc, FaseTypeEnum oldType, FaseTypeEnum newType)
        {
            Fase = fc;
            OldType = oldType;
            NewType = newType;
        }
    }
}
