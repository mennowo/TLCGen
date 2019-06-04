using System;
using TLCGen.Models;

namespace TLCGen.GebruikersOpties
{
    [Serializable]
    public class GebruikersOptieWithIOModel : IOElementModel
    {
        public override string Naam { get; set; }
        public override bool Dummy { get; set; }
        public string Commentaar { get; set; }

        public bool ShouldSerializeCommentaar()
        {
            return !string.IsNullOrWhiteSpace(Commentaar);
        }
    }
}
