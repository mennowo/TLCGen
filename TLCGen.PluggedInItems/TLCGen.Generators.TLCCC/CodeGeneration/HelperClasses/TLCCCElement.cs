using TLCGen.Generators.TLCCC.Settings;

namespace TLCGen.Generators.TLCCC.CodeGeneration
{
    public class TLCCCElement
    {
        public string Define { get; set; }
        public string Naam { get; set; }
        public TLCCCElementTimeTypeEnum TType { get; set; }
        public int? Instelling { get; set; }
        public string Commentaar { get; set; }
        public TLCCCElementTypeEnum Type { get; set; }
        public bool Dummy { get; set; }

        public TLCCCElement()
        {
            Dummy = false;
        }

        public TLCCCElement(string naam, TLCCCElementTypeEnum type)
        {
            Dummy = false;
            Naam = naam;
            Define = TLCCCGeneratorSettingsProvider.Default.GetPrefix(type) + naam;
            Type = type;
            switch (Type)
            {
                default:
                    TType = TLCCCElementTimeTypeEnum.None;
                    break;
            }
        }

        public TLCCCElement(string naam, int instelling, TLCCCElementTimeTypeEnum ttype, TLCCCElementTypeEnum type)
        {
            Dummy = false;
            Naam = naam;
            Define = TLCCCGeneratorSettingsProvider.Default.GetPrefix(type) + naam;
            Instelling = instelling;
            TType = ttype;
            Type = type;
        }
    }
}