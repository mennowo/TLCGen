using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public CCOLElementTypeEnum Type { get; set; }

        public CCOLElement()
        {

        }

        public CCOLElement(string define, string naam, CCOLElementTypeEnum type)
        {
            Define = define;
            Naam = naam;
            Type = type;
            switch(Type)
            {
                case CCOLElementTypeEnum.Schakelaar:
                    TType = CCOLElementTimeTypeEnum.SCH_type;
                    break;
                default:
                    TType = CCOLElementTimeTypeEnum.None;
                    break;
            }
        }

        public CCOLElement(string define, string naam, int instelling, CCOLElementTimeTypeEnum ttype, CCOLElementTypeEnum type)
        {
            Define = define;
            Naam = naam;
            Instelling = instelling;
            TType = ttype;
            Type = type;
        }
    }
}
