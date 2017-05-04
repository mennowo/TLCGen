using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TLCGen.Generators.CCOL.Settings
{
    [Serializable]
    public class CodePieceSettingsTuple<T1, T2>
    {
        CodePieceSettingsTuple() { }

        [XmlAttribute(AttributeName = "CodePiece")]
        public T1 Item1 { get; set; }

        [XmlElement(ElementName = "CodePieceData")]
        public T2 Item2 { get; set; }

        public static implicit operator CodePieceSettingsTuple<T1, T2>(Tuple<T1, T2> t)
        {
            return new CodePieceSettingsTuple<T1, T2>()
            {
                Item1 = t.Item1,
                Item2 = t.Item2
            };
        }

        public static implicit operator Tuple<T1, T2>(CodePieceSettingsTuple<T1, T2> t)
        {
            return Tuple.Create(t.Item1, t.Item2);
        }
    }
}
