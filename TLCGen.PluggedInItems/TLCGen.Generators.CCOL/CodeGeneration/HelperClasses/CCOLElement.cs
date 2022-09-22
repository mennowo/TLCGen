using System;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public class CCOLElement
    {
        public string Define { get; set; }
        public string Naam { get; set; }
        public CCOLElementTimeTypeEnum TType { get; set; }
        public int? Instelling { get; set; }
        public string Commentaar { get; set; }
        public string Categorie { get; set; }
        public string SubCategorie { get; set; }
        public CCOLElementTypeEnum Type { get; set; }
        public bool Dummy { get; set; }
        public int RangeerIndex { get; set; }
        public IOElementModel IOElementData { get; set; }
        public bool IOMultivalent { get; set; }

        public CCOLElement()
        {
            Dummy = false;
        }

        public CCOLElement(string naam, string cat, string subcat, CCOLElementTypeEnum type, string description = null, IOElementModel ioElementData = null)
        {
            Dummy = false;
            Naam = naam;
            Define = CCOLGeneratorSettingsProvider.Default.GetPrefix(type) + naam;
            Type = type;
			Commentaar = description;
            TType = Type switch
            {
                CCOLElementTypeEnum.Schakelaar => CCOLElementTimeTypeEnum.SCH_type,
                _ => CCOLElementTimeTypeEnum.None
            };
            IOElementData = ioElementData;
            Categorie = cat;
            SubCategorie = subcat;
            
            // check
            if ((type == CCOLElementTypeEnum.Uitgang || type == CCOLElementTypeEnum.Ingang) && ioElementData == null)
            {
                throw new NullReferenceException("IO data may not be null for IO");
            }

            // sync data
            if (ioElementData != null)
            {
                Naam ??= ioElementData.Naam;
                ioElementData.Naam ??= Naam;
                if (!Dummy) Dummy = ioElementData.Dummy;
                if (!IOMultivalent) IOMultivalent = ioElementData.Multivalent;
            }
        }

        public CCOLElement(string naam, string cat, string subcat, int instelling, CCOLElementTimeTypeEnum ttype, CCOLElementTypeEnum type, string description = null)
        {
            Dummy = false;
            Naam = naam;
            Define = CCOLGeneratorSettingsProvider.Default.GetPrefix(type) + naam;
            Instelling = instelling;
            TType = ttype;
            Type = type;
			Commentaar = description;
            Categorie = cat;
            SubCategorie = subcat;
        }
    }
}
