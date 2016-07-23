using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TLCGen.DataAccess
{
    public class SerializeT<T>
    {
        #region GZip Serialization

        public bool SerializeGZip(string file, T t)
        {
            bool result = true;
            try
            {
                FileStream fs = new FileStream(file,
                                           FileMode.Create, FileAccess.Write);
                using (var gz = new GZipStream(fs, CompressionMode.Compress))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(gz, t);
                }
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
