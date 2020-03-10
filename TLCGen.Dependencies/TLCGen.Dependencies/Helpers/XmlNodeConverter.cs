using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace TLCGen.Helpers
{
    public static class XmlNodeConverter
    {
        public static T ConvertNode<T>(XmlNode node) where T : class
        {
            var stm = new MemoryStream();

            var stw = new StreamWriter(stm);
            stw.Write(node.OuterXml);
            stw.Flush();

            stm.Position = 0;

            var ser = new XmlSerializer(typeof(T));
            var result = (ser.Deserialize(stm) as T);

            return result;
        }
    }
}
