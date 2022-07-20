using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace TLCGen.Helpers
{
    public static class TLCGenSerialization
    {
        #region Xml Data Serialization

        public static T DeSerializeData<T>(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                return default(T);

            var t = default(T);

            try
            {
                using (TextReader sr = new StringReader(data))
                {
                    var serializer = new XmlSerializer(typeof(T));
                        t = (T)serializer.Deserialize(sr);
                }
            }
            catch (Exception)
            {
                MessageBox.Show($"Data formaat onjuist. Controleer de data.", "Fout bij lezen data.");
            }

            return t;
        }

        #endregion // Xml Data Serialization

        #region Xml File Serialization

        public static bool Serialize<T>(string file, T t)
        {
            var result = true;
            try
            {
                using (var fs = new FileStream(file, FileMode.Create, FileAccess.Write))
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

        public static T DeSerialize<T>(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName) || !File.Exists(fileName))
                return default(T);

            var t = default(T);

            try
            {
                using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    t = (T)serializer.Deserialize(fs);
                    fs.Close();
                }
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show($"Bestandsformaat onjuist (betand: {fileName}).\nIs dit een bestand horend bij type {typeof(T).Name}?", "Fout bij laden bestand.");
            }
            catch (Exception e)
            {
                MessageBox.Show("Fout bij laden bestand: " + e.ToString(), "Fout bij laden bestand.");
            }

            return t;
        }

        #endregion // Xml File Serialization

        #region XmlDocument Serialization

        public static T SerializeFromXmlDocument<T>(XmlDocument doc)
        {
            T t;
            using (var r = XmlReader.Create(new StringReader(doc.InnerXml)))
            {
                var serializer = new XmlSerializer(typeof(T));
                t = (T)serializer.Deserialize(r);
            }
            return t;
        }

        public static XmlDocument SerializeToXmlDocument<T>(T t)
        {
            var doc = new XmlDocument();
            var nav = doc.CreateNavigator();
            using (var w = nav.AppendChild())
            {
                var ser = new XmlSerializer(typeof(T));
                ser.Serialize(w, t);
            }

            return doc;
        }

        #endregion // XmlDocument Serialization

        #region GZip Serialization

        public static T DeSerializeGZip<T>(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName) || !File.Exists(fileName))
                return default(T);

            var t = default(T);

            try
            {
                var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                using (var gz = new GZipStream(fs, CompressionMode.Decompress))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    t = (T)serializer.Deserialize(gz);
                }
                fs.Close();
            }
            catch (InvalidOperationException)
            {
                System.Windows.MessageBox.Show("Bestandsformaat onjuist. Is dit een TLCGen bestand?", "Fout bij laden bestand.");
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("Fout bij laden bestand: " + e.ToString(), "Fout bij laden bestand.");
            }

            return t;
        }

        public static bool SerializeGZip<T>(string file, T t)
        {
            var result = true;
            try
            {
                var fs = new FileStream(file, FileMode.Create, FileAccess.Write);
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
