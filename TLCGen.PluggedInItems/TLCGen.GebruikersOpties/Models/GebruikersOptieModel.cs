using System;

namespace TLCGen.GebruikersOpties
{
    [Serializable]
    public class GebruikersOptieModel
    {
        public string Naam { get; set; }

        public CCOLElementTypeEnum Type { get; set; }
        public int? Instelling { get; set; }
        public bool Dummy { get; set; }
        public string Commentaar { get; set; }

        public bool ShouldSerializeType()
        {
            return Instelling.HasValue;
        }
        public bool ShouldSerializeInstelling()
        {
            return Instelling.HasValue;
        }
        public bool ShouldSerializeCommentaar()
        {
            return !string.IsNullOrWhiteSpace(Commentaar);
        }
    }
}
