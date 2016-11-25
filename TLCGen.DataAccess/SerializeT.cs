using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace TLCGen.DataAccess
{
    public class SerializeT<T>
    {
        #region XML Serialization

        public bool Serialize(string file, T t)
        {
            bool result = true;
            try
            {
                using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(fs, t);
                    fs.Close();
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public XmlDocument SerializeToXmlDocument(T t)
        {
            //XmlSerializer ser = new XmlSerializer(typeof(T));
            //XmlDocument xd = null;
            //
            //using (MemoryStream memStm = new MemoryStream())
            //{
            //    ser.Serialize(memStm, t);
            //    XmlReaderSettings settings = new XmlReaderSettings();
            //    settings.IgnoreWhitespace = true;
            //
            //    using (var xtr = XmlReader.Create(memStm, settings))
            //    {
            //        xd = new XmlDocument();
            //        xd.Load(xtr);
            //    }
            //}

            XmlDocument doc = new XmlDocument();
            XPathNavigator nav = doc.CreateNavigator();
            using (XmlWriter w = nav.AppendChild())
            {
                XmlSerializer ser = new XmlSerializer(typeof(T));
                ser.Serialize(w, t);
            }

            return doc;
        }

        #endregion // XML Serialization

        #region GZip Serialization

        public bool SerializeGZip(string file, T t)
        {
            bool result = true;
            try
            {
                FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write);
                using (var gz = new GZipStream(fs, CompressionMode.Compress))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(gz, t);
                }
                fs.Close();
            }
            catch
            {
                result = false;
            }
            return result;
        }

        #endregion // GZip Serialization
    }
}
