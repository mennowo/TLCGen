using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TLCGen.Helpers
{
    public class DeserializeT<T>
    {
        #region GZip Serialization

        public T DeSerializeGZip(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName) || !File.Exists(fileName))
                return default(T);

            T t = default(T);

            try
            {
                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                using (var gz = new GZipStream(fs, CompressionMode.Decompress))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    t = (T)serializer.Deserialize(gz);
                }
                fs.Close();
            }
            catch
            {
                throw new NotImplementedException();
            }

            return t;
        }

        #endregion // GZip Serialization
    }
}
