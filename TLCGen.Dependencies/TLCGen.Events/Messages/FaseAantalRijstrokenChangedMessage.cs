
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Messaging.Messages
{
	public class FaseAantalRijstrokenChangedMessage
    {
        public FaseCyclusModel Fase { get; }
        public int? AantalRijstroken { get; }

        public FaseAantalRijstrokenChangedMessage(FaseCyclusModel fc, int? aantalRijkstroken)
        {
            Fase = fc;
            AantalRijstroken = aantalRijkstroken;
        }
    }
}
